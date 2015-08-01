using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.TeachersVM
{
    public class TeachersAssignVM
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int SelectedCourseId { get; set; }

        public List<Course> Courses { get; set; }
    }
}