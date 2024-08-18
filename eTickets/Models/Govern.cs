using eTickets.Data.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTickets.Models
{
    public class Govern:IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Governorate")]

        public string Name { get; set; }

        public List<Cinema> Cinemas { get; set; }
    }
}
