﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.ArticlesVM
{
    public class ArticlesCreateVM
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        public IEnumerable<SelectListItem> Subjects { get; set; }

        [Display(Name="Subject")]
        public int SubjectId { get; set; }
    }
}