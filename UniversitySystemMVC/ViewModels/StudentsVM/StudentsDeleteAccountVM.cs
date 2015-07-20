using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.StudentsVM
{
    public class StudentsDeleteAccountVM
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string FacultyNumber { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}