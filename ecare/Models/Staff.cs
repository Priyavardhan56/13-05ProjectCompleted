using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ecare.Models
{
    public class Staff
    {
        [ScaffoldColumn(false)]
        public int StaffId { get; set; }



        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter name"), MaxLength(30)]
        public string StaffName { get; set; }



        [Required(ErrorMessage = "StaffPhoto Can't Be Empty")]
        public HttpPostedFileBase StaffPhoto { get; set; }



        [Required(ErrorMessage = "This field is requied")]
        public int HospitalId { get; set; }



        [Required(ErrorMessage = "EmployeeCode Can't Be Empty")]
        public int EmployeeCode { get; set; }



        [Required(ErrorMessage = "StaffSpecilization Can't Be Empty")]
        public string StaffSpecialization { get; set; }



        [Required(ErrorMessage = "StaffDegree Can't Be Empty")]
        public string StaffDegree { get; set; }



        [Required(ErrorMessage = "Designation Can't Be Empty")]
        public string Designation { get; set; }



        [Required(ErrorMessage = "Please enter Mobile No")]
        //[Display(Name = "")]
        [DataType(DataType.PhoneNumber)]
        public string StaffPhone { get; set; }



        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter Email ID")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string StaffEmail { get; set; }



        [Required(ErrorMessage = "City Can't Be Empty")]
        public string StaffCity { get; set; }



        [Required(ErrorMessage = "State Can't Be Empty")]
        public string StaffState { get; set; }



        [Required(ErrorMessage = "Country")]
        public string StaffCountry { get; set; }



        [Required(ErrorMessage = "This field is requied")]
        public bool IsActive { get; set; }



        [Required(ErrorMessage = "This field is requied")]
        public string EntryDate { get; set; }



        public string EntryBy { get; set; }



        [Required(ErrorMessage = "RoleId")]
        public int RoleId { get; set; }



        [Required(ErrorMessage = "StaffAddress Can't Be Empty")]
        public string StaffAddress { get; set; }


        public string Password { get; set; }
        public string ActionType { get; set; }

    }
}