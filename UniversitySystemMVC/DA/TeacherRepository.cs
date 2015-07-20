using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class TeacherRepository : UserRepository<Teacher>
    {
        public TeacherRepository(AppContext context) : base(context) { }

        public Teacher GetById(int id, bool pullCoursesSubjects)
        {
            var query = dbSet.Where(t => t.Id == id && t.IsActive);
            if (pullCoursesSubjects)
            {
                query = query.Include(t => t.CourseSubjects);
            }

            return query.FirstOrDefault();
        }

        public override IEnumerable<Teacher> GetAll()
        {
            return dbSet.Where(t => t.IsActive).ToList();
        }
    }
}