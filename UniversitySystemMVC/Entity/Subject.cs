using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Subject : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<Grade> Grades { get; set; }
        public ICollection<CoursesSubjects> CoursesSubjects { get; set; }
        //public virtual ICollection<SubjectCourseTeacher> SubjectCourseTeachers { get; set; }
    }
}