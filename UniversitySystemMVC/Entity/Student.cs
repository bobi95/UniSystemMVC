using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Student : User
    {
        public string FacultyNumber { get; set; }

        public int? CourseId { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
    }
}