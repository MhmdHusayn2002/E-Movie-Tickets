using eTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    public interface IOrdersService
    {
        Task<bool> StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress, DateTime movieDate);
        Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole);
    }
}
