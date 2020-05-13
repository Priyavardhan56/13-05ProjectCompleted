using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ecare.Models
{
    public class Contact
    {
        [Display(Name = "Id")]

        [ScaffoldColumn(false)]
        public int Id { get; set; }



        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter name"), MaxLength(30)]
        public string Name { get; set; }



        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter Email ID")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }



        [Required(ErrorMessage = "Please enter Mobile No")]
        [Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }


        [Required(ErrorMessage = "This field is requied")]
        public string Message { get; set; }


        public string EntryDateTime { get; set; }


    } }