using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.ViewModels.TeachersVM
{
    public class TeachersConfirmAccountVM
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        [Display(Name = "Re-New Password")]
        public string NewPasswordRe { get; set; }
    }
}