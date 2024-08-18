using eTickets.Data;
using eTickets.Data.Services;
using eTickets.Data.Static;
using eTickets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class ProducersController : Controller
    {
        AppDbContext db;
        IWebHostEnvironment _hostingEnvironment;

        public ProducersController(AppDbContext db, IWebHostEnvironment _hostingEnvironment)
        {
            this.db= db;    
            this._hostingEnvironment = _hostingEnvironment; 
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            List<Producer> producers = db.Producers.ToList();
            return View(producers);
        }

        //GET: producers/details/1
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            Producer producerDetails = db.Producers.Find(id);
            if (producerDetails == null) return View("NotFound");
            return View(producerDetails);
        }

        //GET: producers/create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Producer producer, IFormFile ProfilePictureURL)
        {
            if (!ModelState.IsValid) return View(producer);
            if (ProfilePictureURL != null && ProfilePictureURL.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Path.GetFileName(ProfilePictureURL.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePictureURL.CopyTo(fileStream);
                }

                // Set the ProfilePictureURL property of the Actor object
                producer.ProfilePictureURL = "/images/" + fileName;
            }

            db.Producers.Add(producer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: producers/edit/1
        public IActionResult Edit(int id)
        {
            Producer producerDetails = db.Producers.Find(id);
            if (producerDetails == null) return View("NotFound");
            return View(producerDetails);
        }

        [HttpPost]
        public IActionResult Edit(int id,Producer producer, IFormFile ProfilePictureURL)
        {
            if (id != producer.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(producer);
            }

            string oldFilePath = null;
            if (!string.IsNullOrEmpty(producer.ProfilePictureURL))
            {
                oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", producer.ProfilePictureURL.TrimStart('/'));
            }

            if (ProfilePictureURL != null && ProfilePictureURL.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Path.GetFileName(ProfilePictureURL.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePictureURL.CopyTo(fileStream);
                }

                // Delete the old profile picture file from the server
                if (!string.IsNullOrEmpty(oldFilePath) && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Set the ProfilePictureURL property of the Actor object
                producer.ProfilePictureURL = "/images/" + fileName;
            }





            db.Producers.Update(producer);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        //GET: producers/delete/1
        public IActionResult Delete(int id)
        {
            Producer producerDetails = db.Producers.Find(id);
            if (producerDetails == null) return View("NotFound");
            return View(producerDetails);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            Producer producerDetails = db.Producers.Find(id);
            if (producerDetails == null) return View("NotFound");

            db.Producers.Remove(producerDetails);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
