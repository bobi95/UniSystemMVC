using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.CoursesVM
{
    public class CoursesCreateVM
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public int CourseCode { get; set; }

        public List<Subject> Subjects { get; set; }
    }
}