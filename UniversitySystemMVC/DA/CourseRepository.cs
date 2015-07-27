using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class CourseRepository : BaseRepository<Course>
    {
        public CourseRepository(AppContext context) : base(context) { }

        //public List<Subject> GetAll(bool pullCourseSubjects = false)
        //{
        //    var query = dbSet.AsQueryable<Subject>().ToList();
        //    if (pullCourseSubjects)
        //    {
        //        var cs = context.Set<CoursesSubjects>().AsQueryable<CoursesSubjects>().Include(s => s.Course).Include(s => s.Teachers).ToList();

        //        foreach (var q in query)
        //        {
        //            q.CoursesSubjects = cs.Where(x => x.SubjectId == q.Id).ToList();
        //        }
        //    }

        //    return query;
        //} 

        public bool CheckIfCourseCodeExists(int courseCode)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            foreach (var c in unitOfWork.CourseRepository.GetAll())
            {
                if (c.Code == courseCode)
                {
                    return true;
                }
            }

            return false;
        }
    }
}