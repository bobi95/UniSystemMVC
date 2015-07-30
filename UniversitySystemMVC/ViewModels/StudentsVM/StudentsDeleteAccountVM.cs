using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.StudentsVM
{
    public class StudentsDeleteAccountVM
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

        [Display(Name = "Course")]
        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}