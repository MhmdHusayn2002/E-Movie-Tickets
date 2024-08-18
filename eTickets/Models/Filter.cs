using System.Collections.Generic;

namespace eTickets.Models
{
    public class Filter
    {
        public IEnumerable<Movie> Movies { get; set; }
        public IEnumerable<Cinema> Cinemas { get; set; }
        public IEnumerable<Govern> Governs { get; set; }
        public string SearchType { get; set; }
    }
}
