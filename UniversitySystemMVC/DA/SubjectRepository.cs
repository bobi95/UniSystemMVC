using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class SubjectRepository : BaseRepository<Subject>
    {
        public SubjectRepository(AppContext context) : base(context) { }

        public List<Subject> GetAll(bool pullCourseSubjects = false)
        {
            var query = dbSet.AsQueryable<Subject>().ToList();
            if (pullCourseSubjects)
            {
                var cs = context.Set<CoursesSubjects>().AsQueryable<CoursesSubjects>().Include(s => s.Course).Include(s => s.Teachers).ToList();

                foreach (var item in cs)
                {
                    item.Teachers = item.Teachers.Where(t => t.IsActive).ToList();
                }

                foreach (var q in query)
                {
                    q.CoursesSubjects = cs.Where(x => x.SubjectId == q.Id).ToList();
                }
            }

            return query;
        }

        public Subject GetById(int id, bool pullGrades = false)
        {
            var query = dbSet.Where(s => s.Id == id);

            if (pullGrades)
            {
                query = query.Include(s => s.Grades);
            }

            return query.FirstOrDefault();
        }
    }
}