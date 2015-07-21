using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.CoursesVM
{
    public class CoursesDetailsVM
    {
        public Course Course { get; set; }

        public List<Student> Students { get; set; }
    }
}