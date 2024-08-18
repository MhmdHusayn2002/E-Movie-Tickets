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
    public class GovernsController : Controller
    {
        AppDbContext db;
        public GovernsController(AppDbContext db)

        {
            this.db = db;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            List<Govern> governs = db.Governs.ToList();
            return View(governs);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var governDetails = await db.Governs.Include(c => c.Cinemas).FirstOrDefaultAsync(c => c.Id == id);
            if (governDetails == null) return View("NotFound");
            return View(governDetails);
        }
        //Get: Governs/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Govern govern)
        {
            if (!ModelState.IsValid) return View(govern);
            db.Governs.Add(govern);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Get: Governs/Details/1
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            Govern governDetails = db.Governs.Find(id);
            if (governDetails == null) return View("NotFound");
            return View(governDetails);
        }

        //Get: Governs/Edit/1
        public IActionResult Edit(int id)
        {
            Govern governDetails = db.Governs.Find(id);
            if (governDetails == null) return View("NotFound");
            return View(governDetails);
        }

        [HttpPost]
        public IActionResult Edit(int id, Govern govern)
        {
            if (id != govern.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(govern);
            }
            db.Governs.Update(govern);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Get: Governs/Delete/1
        public IActionResult Delete(int id)
        {
            Govern governDetails = db.Governs.Find(id);
            if (governDetails == null) return View("NotFound");
            return View(governDetails);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirm(int id)
        {
            Govern governDetails = db.Governs.Find(id);
            if (governDetails == null) return View("NotFound");
            db.Governs.Remove(governDetails);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
