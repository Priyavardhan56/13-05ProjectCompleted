using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ecare.Models
{
    public class Appointment
    {
        [Display(Name = "AppointmentId")]
        [ScaffoldColumn(false)]
        public int AppointmentId { get; set; }


        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter name"), MaxLength(30)]
        public string Name { get; set; }


        [Required]
        public int Age { get; set; }


        [Required(ErrorMessage = "This field is requied")]
        public string Gender { get; set; }



        [Required(ErrorMessage = "This field is requied")]
        public string AppointmentType { get; set; } = "Online";


        [Required]
        [StringLength(1000)]
        public string Address { get; set; }


        [Required(ErrorMessage = "Please enter Mobile No")]
        //[Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }


        [Required]
        [StringLength(1000)]
        public string Problem { get; set; }


      
        public string Payment { get; set; } = "NotPaid";


        [Required]
        public string Date { get; set; }

        [Required]
        public string Slot { get; set; }

        [Required]
        public int HospitalId { get; set; }


        [Required]
        public string EntryBy { get; set; }

        [Required]
        public string EntryDateTime { get; set; }


        public string Doctor { get; set; } = "NotAssigned";


      
    }


}