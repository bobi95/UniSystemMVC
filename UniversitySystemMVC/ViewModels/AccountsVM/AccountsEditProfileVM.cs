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
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        public string NewPasswordRe { get; set; }

        [Required]
        [EmailAddressAttribute]
        public string Email { get; set; }
    }
}