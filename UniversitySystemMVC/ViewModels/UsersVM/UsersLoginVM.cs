using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.UsersVM
{
    public class UsersLoginVM
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}