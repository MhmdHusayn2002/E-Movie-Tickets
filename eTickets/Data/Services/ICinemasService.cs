﻿using eTickets.Data.Base;
using eTickets.Data.ViewModels;
using eTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    public interface ICinemasService:IEntityBaseRepository<Cinema>
    {
        Task<Cinema> GetCinemaByIdAsync(int id);
    }
}
