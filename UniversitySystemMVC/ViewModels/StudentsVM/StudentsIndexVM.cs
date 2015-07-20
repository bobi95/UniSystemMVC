using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.StudentsVM
{
    public class StudentsIndexVM
    {
        public Student Student { get; set; }

        public Course Course { get; set; }

        public List<CoursesSubjects> CoursesSubjects { get; set; }

        public List<Grade> Grades { get; set; }
    }
}