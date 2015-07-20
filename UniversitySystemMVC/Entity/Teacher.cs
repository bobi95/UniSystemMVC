using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Teacher : User
    {
        public int? TitleId { get; set; }
        public virtual Title Title { get; set; }

        public virtual List<CoursesSubjects> CourseSubjects { get; set; }
        //public virtual ICollection<SubjectCourseTeacher> SubjectCourseTeachers { get; set; }
    }
}