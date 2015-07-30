using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.ViewModels.CoursesVM
{
    public class CoursesDeleteVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Course Code")]
        public int CourseCode { get; set; }
    }
}