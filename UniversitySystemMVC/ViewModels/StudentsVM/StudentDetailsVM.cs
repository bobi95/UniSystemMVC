using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.StudentsVM
{
    public class StudentDetailsVM
    {
        public int Id { get; set; }

        public string Username { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Faculty Number")]
        public string FacultyNumber { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Is Confirmed")]
        public bool IsConfirmed { get; set; }

        public Course Course { get; set; }

        public List<CoursesSubjects> CoursesSubjects { get; set; }

        public Dictionary<string, object> Props { get; set; }

        public string SortOrder { get; set; }
    }
}