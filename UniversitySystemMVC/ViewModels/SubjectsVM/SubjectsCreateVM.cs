using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.SubjectsVM
{
    public class SubjectsCreateVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Course> Courses { get; set; }
    }
}