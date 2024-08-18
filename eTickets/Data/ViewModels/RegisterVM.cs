using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.ViewModels
{
    public class RegisterVM
    {
        //[Required]
       //public byte[] ProfilePicture { get; internal set; }
        
        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(maximumLength: 10, MinimumLength = 3, ErrorMessage = "Username musst have max length 10 and min length 3")]
        public string FullName { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Enter The Proper Email Addresss !")]
        public string EmailAddress { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        //[RegularExpression("^(?=.*[A-Za-z\\d](?=.*\\d)[A-Za-z\\d]{8,}$", ErrorMessage = "Password must contain atleast 8 Characters and must haave 1 alphabat and 1 number")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The password must be at least {0} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",ErrorMessage = "Password must contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character.")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
