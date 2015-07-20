using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Course : BaseEntity
    {
        public string Name { get; set; }
        public int Code { get; set; }

        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<CoursesSubjects> CoursesSubjects { get; set; }
        //public virtual ICollection<SubjectCourseTeacher> SubjectCourseTeachers { get; set; }
    }
}