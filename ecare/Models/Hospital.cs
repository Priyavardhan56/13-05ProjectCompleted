using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ecare.Models
{
    public class Hospital
    {
        [Display(Name = "HospitalId")]
        [ScaffoldColumn(false)]
        public int HospitalId { get; set; }


        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter name"), MaxLength(30)]
        public string HospitalName { get; set; }



        [Required(ErrorMessage = "This field is requied")]
        public string HospitalAddress { get; set; }


        [Required(ErrorMessage = "This field is requied")]
        public string HospitalCity { get; set; }


        [Required(ErrorMessage = "This field is requied")]
        public string HospitalState { get; set; }


        [Required(ErrorMessage = "This field is requied")]
        public string HospitalCountry { get; set; }



        [Required(ErrorMessage = "Please enter Mobile No")]
        //[Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        public string HospitalPhone { get; set; }

    

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter Email ID")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string HospitalEmail { get; set; }


        public string HospitalLogo { get; set; }


        [Required(ErrorMessage = "This field is requied")]
        public bool IsActive { get; set; }

 
        public string Password { get; set; }
      
        public string EntryDateTime { get; set; }

        public string EntryBy { get; set; }

        public string ActionType { get; set; }
        public int HistId { get; set; }

    }

}