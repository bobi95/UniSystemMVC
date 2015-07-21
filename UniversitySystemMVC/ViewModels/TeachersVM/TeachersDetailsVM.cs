using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.TeachersVM
{
    public class TeachersDetailsVM
    {
        public Teacher Teacher { get; set; }

        public List<CoursesSubjects> CoursesSubjects { get; set; }
    }
}