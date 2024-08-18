using eTickets.Models;
using eTickets.Data.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    public class GovernsService :EntityBaseRepository<Govern>, IGovernsService
    {
        private readonly AppDbContext _context;
        public GovernsService(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Govern> GetGovernByIdAsync(int id)
        {
            return await _context.Governs.FindAsync(id);
        }
    }
}
