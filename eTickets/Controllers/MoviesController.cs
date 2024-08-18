using eTickets.Data;
using eTickets.Data.Services;
using eTickets.Data.Static;
using eTickets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Controllers
{
    

    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMoviesService _service;
        
        private readonly IWebHostEnvironment _environment;




        public MoviesController(IMoviesService service,IWebHostEnvironment environment,AppDbContext context)
        {
            _service = service;
            _environment = environment;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allMovies = await _service.GetAllAsync(n => n.Cinema);
            return View(allMovies);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString, MovieCategory? movieCategory)
        {
            var allMovies = await _service.GetAllAsync(n => n.Cinema);

            if (!string.IsNullOrEmpty(searchString))
            {
                var filteredResultNew = allMovies.Where(n => string.Equals(n.Name, searchString, StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(n.Description, searchString, StringComparison.CurrentCultureIgnoreCase)
                    || (n.MovieCategory.ToString() != null && string.Equals(n.MovieCategory.ToString(), searchString, StringComparison.CurrentCultureIgnoreCase))).ToList();

                return View("Index", filteredResultNew);
            }

            if (movieCategory.HasValue)
            {
                var filteredResultByCategory = allMovies.Where(n => n.MovieCategory == movieCategory).ToList();
                return View("Index", filteredResultByCategory);
            }

            return View("Index", allMovies);
        }


        //GET: Movies/Details/1
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var movieDetail = await _service.GetMovieByIdAsync(id);
            return View(movieDetail);
        }

        //GET: Movies/Create
        public async Task<IActionResult> Create()
        {
            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewMovieVM movie , IFormFile poster_img)
        {
            



            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
                string path = Path.Combine(_environment.WebRootPath, "poster_img");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (poster_img != null)
                {
                

                    path = Path.Combine(path, poster_img.FileName);
                    
                    // for exmple : /Img/Photoname.png
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await poster_img.CopyToAsync(stream);

                        movie.ImageURL = "\\poster_img\\"+ poster_img.FileName;
                        //movie.ImageURL = "~\\wwwroot\\poster_img\\" + poster_img.FileName;
                    }
                }
                else
                {
                    movie.ImageURL = "default.jpeg"; // to save the default image path in database.
                }
                try
                {
                    await _service.AddNewMovieAsync(movie);

                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex) { ViewBag.exc = ex.Message; }

                return View(movie);
            }

            await _service.AddNewMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }


        //GET: Movies/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var movieDetails = await _service.GetMovieByIdAsync(id);
            if (movieDetails == null) return View("NotFound");

            var response = new NewMovieVM()
            {
                Id = movieDetails.Id,
                Name = movieDetails.Name,
                Description = movieDetails.Description,
                Price = movieDetails.Price,
                StartDate = movieDetails.StartDate,
                EndDate = movieDetails.EndDate,
                ShowTime = movieDetails.ShowTime,
                ImageURL = movieDetails.ImageURL,
                MovieCategory = movieDetails.MovieCategory,
                AvailableTickets = movieDetails.AvailableTickets,
                CinemaId = movieDetails.CinemaId,
                ProducerId = movieDetails.ProducerId,
                ActorIds = movieDetails.Actors_Movies.Select(n => n.ActorId).ToList(),
            };

            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();
            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

            return View(response);
        }
        ////////////////////////////////////////////////
        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewMovieVM movie, IFormFile poster_img)
        {
            if (id != movie.Id) return View("NotFound");

            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

                string path = Path.Combine(_environment.WebRootPath, "poster_img");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (poster_img != null)
                {


                    path = Path.Combine(path, poster_img.FileName);

                    // for exmple : /Img/Photoname.png
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await poster_img.CopyToAsync(stream);

                        movie.ImageURL = "\\poster_img\\" + poster_img.FileName;
                        //movie.ImageURL = "~\\wwwroot\\poster_img\\" + poster_img.FileName;
                    }
                }
                else
                {
                    movie.ImageURL = "default.jpeg"; // to save the default image path in database.
                }
                try
                {
                    await _service.UpdateMovieAsync(movie);
                  //  await _service.AddNewMovieAsync(movie);

                    _context.SaveChanges();

                    //_context.AddRangeAsync(movie, poster_img);
                    // _context.SaveChanges();


                    return RedirectToAction("Index");
                }
                catch (Exception ex) { ViewBag.exc = ex.Message; }
                return View(movie);
            }

            
            return RedirectToAction(nameof(Index));
        }
        //Get: Movies/Delete/1
        public IActionResult Delete(int id)
        {
            Movie movieDetails = _context.Movies.Find(id);
            if (movieDetails == null) return View("NotFound");
            return View(movieDetails);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirm(int id)
        {
            Movie movieDetails = _context.Movies.Find(id);
            if (movieDetails == null) return View("NotFound");

            _context.Movies.Remove(movieDetails);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
