using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.AdminsVM
{
    public class AdminsCreateAccountVM
    {
        public int Id { get; set; }

        public string Username { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddressAttribute]
        public string Email { get; set; }

        public UserTypeEnum UserType { get; set; }
    }
}