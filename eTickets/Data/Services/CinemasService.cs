using eTickets.Data.Base;
using eTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    public class CinemasService:EntityBaseRepository<Cinema>, ICinemasService
    {
        private readonly AppDbContext _context;
        public CinemasService(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Cinema> GetCinemaByIdAsync(int id)
        {
            return await _context.Cinemas.FindAsync(id);
        }
    }
}
