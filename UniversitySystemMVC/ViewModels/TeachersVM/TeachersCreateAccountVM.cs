﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.TeachersVM
{
    public class TeachersCreateAccountVM
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

        [Display(Name = "Title")]
        public int TitleId { get; set; }

        public IEnumerable<SelectListItem> Titles { get; set; }

        public List<Subject> Subjects { get; set; }

        public UserTypeEnum UserType { get; set; }
    }
}