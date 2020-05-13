using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ecare.Models
{
    public class Role
    {
        [ScaffoldColumn(false)]
        public int RoleId { get; set; }



        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter name"), MaxLength(30)]
        public string RoleName { get; set; }



        [Required(ErrorMessage = "RoleDescription Can't Be Empty")]
        public string RoleDescription { get; set; }



        public string EntryBy { get; set; }


        public string EntryDateTime { get; set; }


    }
}