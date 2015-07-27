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

        public override IEnumerable<Student> GetAll(System.Linq.Expressions.Expression<Func<Student, bool>> predicate = null)
        {
            return base.GetAll().OrderBy(s => s.FacultyNumber);
        }
        public List<Student> GetByCourseId(int id)
        {
            return GetAll(null, false).Where(s => s.CourseId == id).OrderBy(s => s.FacultyNumber).ToList();
        }
    }
}