using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.StudentsVM
{
    public class StudentsIndexVM
    {
        public Course Course { get; set; }

        public List<Grade> Grades { get; set; }
    }
}