using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ecare.Models
{
    public class User
    {
        [Display(Name = "Id")]

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter Confirm Password")]
        [Compare("Password", ErrorMessage = "password doesn't match")]
        public string ConfirmPassword { get; set; }

        public string EntryBy { get; set; } = "UnKnown";


    }
    public class Credentials
    {

        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }


        //public int RoleId { get; set; }

    }

    public class ForgotPassword
    {
        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; }
    }
}