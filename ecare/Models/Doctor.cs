using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ecare.Models
{
    public class Doctor
    {
        [Display(Name = "DoctorId")]


        public int DoctorId { get; set; }

        [Required(ErrorMessage = "This field is requied")]
        public string EmployeeCode { get; set; }
        [Required(ErrorMessage = "This field is requied")]
        public HttpPostedFileBase DoctorPhoto { get; set; }
        [Required(ErrorMessage = "This field is requied")]

        public string DoctorName { get; set; }
        [Required(ErrorMessage = "This field is requied")]
        public int HospitalId { get; set; }
        [Required(ErrorMessage = "This field is requied")]
        public string DoctorSpecialization { get; set; }
        [Required(ErrorMessage = "This field is requied")]
        public string DoctorDegree { get; set; }
      

        //[Display(Name = "Mobile Number:")]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string DoctorPhone { get; set; }
        [Required(ErrorMessage = "This field is requied")]
        public string DoctorEmail { get; set; }
        [Required(ErrorMessage = "This field is requied")]
        public string DoctorCity { get; set; }
        [Required(ErrorMessage = "This field is requied")]
        public string DoctorState { get; set; }
        [Required(ErrorMessage = "This field is requied")]

        public string DoctorCountry { get; set; }
        [Required(ErrorMessage = "This field is requied")]
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "This field is requied")]
        public string EntryDate { get; set; }

        public string EntryBy { get; set; }

        [Required(ErrorMessage = "This field is requied")]
        public string DoctorAddress { get; set; }
        public string Password { get; set; }
        public string ActionType { get; set; }


    }
}