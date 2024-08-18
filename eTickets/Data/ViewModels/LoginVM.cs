using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Enter The Proper Email Addresss !")]
        public string EmailAddress { get; set; }

        
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The password must be at least {0} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",ErrorMessage = "Password must contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character.")]
        public string Password { get; set; }
    }
}
