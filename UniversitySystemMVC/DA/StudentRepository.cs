using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class StudentRepository : UserRepository<Student>
    {
        public StudentRepository(AppContext context) : base(context) { }

        public List<Student> GetByCourseId(int id)
        {
            return GetAll(null, false).Where(s => s.CourseId == id).ToList();
        }
    }
}