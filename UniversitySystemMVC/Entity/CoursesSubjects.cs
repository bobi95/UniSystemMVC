using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class CoursesSubjects : BaseEntity
    {
        public int CourseId { get; set; }
        public int SubjectId { get; set; }

        public Course Course { get; set; }
        public Subject Subject { get; set; }

        public ICollection<Teacher> Teachers { get; set; }
    }
}