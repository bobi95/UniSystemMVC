using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.SubjectsVM
{
    public class SubjectsDetailsVM
    {
        public Subject Subject { get; set; }

        public List<CoursesSubjects> CoursesSubjects { get; set; }

        public Dictionary<int, double> SubjectAverages { get; set; }

        public Dictionary<string, object> Props { get; set; }

        public string SortOrder { get; set; }
    }
}