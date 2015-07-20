using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class SubjectCourseTeacher
    {
        [Key]
        [Column(Order=0)]
        public int SubjectId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int CourseId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int TeacherId { get; set; }

        public virtual Subject Subject { get; set; }
        public virtual Course Course { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}