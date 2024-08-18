using eTickets.Data;
using eTickets.Data.Cart;
using eTickets.Data.Services;
using eTickets.Data.Static;
using eTickets.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eTickets.Controllers
{
    [Authorize] 
    public class OrdersController : Controller
    {
        private readonly IMoviesService _moviesService;
        private readonly ShoppingCart _shoppingCart;
        private readonly IOrdersService _ordersService;
        private readonly AppDbContext _context;

        public OrdersController(IMoviesService moviesService, ShoppingCart shoppingCart, IOrdersService ordersService, AppDbContext context)
        {
            _moviesService = moviesService;
            _shoppingCart = shoppingCart;
            _ordersService = ordersService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await _ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            return View(orders);
        }

        public IActionResult ShoppingCart()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCart.UserId = userId;
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            
            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };
            ViewData["CartItemsCount"] = items.Count;
            return View(response);
        }

        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCart.UserId = userId;
            var item = await _moviesService.GetMovieByIdAsync(id);


            if (item != null && item.AvailableTickets > 0)
            {
                //item.AvailableTickets--;
                _context.SaveChanges();
                _shoppingCart.AddItemToCart(item);
            }

            ViewData["CartItemsCount"] = _shoppingCart.GetShoppingCartItems().Count;
            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCart.UserId = userId;
            var item = await _moviesService.GetMovieByIdAsync(id);

            if (item != null)
            {
                //item.AvailableTickets++;
                _context.SaveChanges();
                _shoppingCart.RemoveItemFromCart(item);
            }

            ViewData["CartItemsCount"] = _shoppingCart.GetShoppingCartItems().Count;
            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> CompleteOrder(Payment payment, DateTime MovieDate, int id)
        {
            if (!ModelState.IsValid)
            {
                return View("ShoppingCart");
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

            _shoppingCart.UserId = userId;
            var items = _shoppingCart.GetShoppingCartItems();
            if (MovieDate < DateTime.Now)
            {
                ModelState.AddModelError("MovieDate", "The movie date cannot be in the past.");
                return RedirectToAction(nameof(ShoppingCart));
            }
            if (items.Count == 0)
            {
                // Handle case where cart is empty (optional)
                return RedirectToAction(nameof(ShoppingCart));
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in items)
                    {
                        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == item.Movie.Id);
                        if (movie == null || MovieDate > movie.EndDate)
                        {
                            throw new InvalidOperationException("One or more movies in your cart have expired release dates and cannot be booked.");
                        }
                        if (MovieDate >= DateTime.Now && MovieDate < movie.StartDate)
                        {
                            throw new InvalidOperationException("One or more movies in your cart have not available and cannot be booked.");
                        }

                        if (movie.AvailableTickets < item.Amount)
                        {
                            throw new InvalidOperationException("Not enough available tickets for movie: " + movie.Name);
                        }

                        movie.AvailableTickets -= item.Amount;
                        _context.Movies.Update(movie);
                    }

                    await _ordersService.StoreOrderAsync(items, userId, userEmailAddress, MovieDate);
                    await _shoppingCart.ClearShoppingCartAsync();
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    ViewData["CartItemsCount"] = 0;
                    return View("CompleteOrder");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ViewBag.ErrorMessage = ex.Message;
                    return View("ErrorOrder");
                }
            }

        }
    }
}
