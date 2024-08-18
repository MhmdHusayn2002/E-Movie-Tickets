using eTickets.Data.Base;
using eTickets.Models;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    public interface IGovernsService:IEntityBaseRepository<Govern>
    {
        Task<Govern> GetGovernByIdAsync(int id);
    }
}
