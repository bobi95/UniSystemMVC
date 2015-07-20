using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.TeachersVM
{
    public class TeachersAssignVM
    {
        public int Id { get; set; }

        public int SelectedCourseId { get; set; }

        public List<Course> Courses { get; set; }
    }
}