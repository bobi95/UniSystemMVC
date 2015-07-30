using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.ViewModels.AccountsVM
{
    public class AccountsEditProfileVM
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [Display(Name="Old Password")]
        public string OldPassword { get; set; }

        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        [Display(Name = "Re-New Password")]
        public string NewPasswordRe { get; set; }

        [Required]
        [EmailAddressAttribute]
        public string Email { get; set; }
    }
}