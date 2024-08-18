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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace eTickets.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class CinemasController : Controller
    {
        AppDbContext db;
        IWebHostEnvironment _hostingEnvironment;
        private readonly IGovernsService _governsService;
        private readonly ICinemasService _cinemaService;
        private readonly IMoviesService _moviesService;

        public CinemasController(ICinemasService service, IGovernsService governsService, IMoviesService moviesService, AppDbContext db, IWebHostEnvironment _hostingEnvironment)
        {
            this.db = db;
            this._hostingEnvironment = _hostingEnvironment;
            this._cinemaService = service;
            this._moviesService = moviesService;
            this._governsService = governsService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var cinema = await db.Cinemas.Include(c => c.Governorate).ToListAsync();
            return View(cinema);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString, string searchType)
        {
            var viewModel = new Filter
            {
                SearchType = searchType
            };

            if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchType))
            {
                // Populate default data
                viewModel.Cinemas = await _cinemaService.GetAllAsync();
                return View("Index", viewModel);
            }

            if (searchType == "movie")
            {
                var movies = await _moviesService.GetAllAsync();
                viewModel.Movies = movies.Where(m => m.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else if (searchType == "cinema")
            {
                var cinemas = await _cinemaService.GetAllAsync();
                var filteredCinemas = cinemas.Where(c => c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                viewModel.Cinemas = filteredCinemas;

                // Get movies for the filtered cinemas
                var cinemaIds = filteredCinemas.Select(c => c.Id).ToList();
                var allMovies = await _moviesService.GetAllAsync();
                viewModel.Movies = allMovies.Where(m => cinemaIds.Contains(m.CinemaId)).ToList();
            }
            else if (searchType == "govern")
            {
                var governs = await _governsService.GetAllAsync();
                var filteredGoverns = governs.Where(g => g.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                viewModel.Governs = filteredGoverns;

                var governIds = filteredGoverns.Select(g => g.Id).ToList();
                var allCinemas = await _cinemaService.GetAllAsync();
                viewModel.Cinemas = allCinemas.Where(c => governIds.Contains(c.GovernId)).ToList();
            }

            return View("Search", viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Search()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var cinemaDetails = await db.Cinemas.Include(c => c.Movies).FirstOrDefaultAsync(c => c.Id == id);
            if (cinemaDetails == null) return View("NotFound");
            return View(cinemaDetails);
        }

        //

        //Get: Cinemas/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Gov"] = db.Governs.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile Logo)
        {
            if (!ModelState.IsValid) return View(cinema);



            if (Logo != null && Logo.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Path.GetFileName(Logo.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Logo.CopyTo(fileStream);
                }

                // Set the ProfilePictureURL property of the Actor object
                cinema.Logo = "/images/" + fileName;
            }


            var gov = db.Governs.Find(cinema.GovernId);
            var NewCinema = new Cinema
            {
                Logo = cinema.Logo,
                Name = cinema.Name,
                Description = cinema.Description,
                Governorate = gov
            };
            db.Cinemas.Add(NewCinema);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Get: Cinemas/Details/1
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");
            return View(cinemaDetails);
        }

        //Get: Cinemas/Edit/1
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            ViewData["Gov"] = db.Governs.ToList();
            if (cinemaDetails == null) return View("NotFound");
            return View(cinemaDetails);
        }

        [HttpPost]
        public IActionResult Edit(int id, Cinema cinema, IFormFile Logo)
        {
            if (id != cinema.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["Gov"] = db.Governs.ToList(); // Ensure dropdown is populated if the model is invalid
                return View(cinema);
            }

            var existingCinema = db.Cinemas.Find(id);
            if (existingCinema == null)
            {
                return NotFound();
            }

            string oldFilePath = null;
            if (!string.IsNullOrEmpty(existingCinema.Logo))
            {
                oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", existingCinema.Logo.TrimStart('/'));
            }

            if (Logo != null && Logo.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Logo.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Logo.CopyTo(fileStream);
                }

                // Delete the old logo file from the server
                if (!string.IsNullOrEmpty(oldFilePath) && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Update the logo property of the existing cinema object
                existingCinema.Logo = "/images/" + fileName;
            }

            // Update other properties
            existingCinema.Name = cinema.Name;
            existingCinema.Description = cinema.Description;
            existingCinema.GovernId = cinema.GovernId;

            db.Cinemas.Update(existingCinema);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Get: Cinemas/Delete/1
        public IActionResult Delete(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");
            return View(cinemaDetails);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirm(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");

            db.Cinemas.Remove(cinemaDetails);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
