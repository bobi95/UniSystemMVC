using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using UniversitySystemMVC.Entity;
using System.Linq.Expressions;

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

        public override IEnumerable<Teacher> GetAll(Expression<Func<Teacher, bool>> predicate = null)
        {
            return dbSet.Where(t => t.IsActive).ToList();
        }

        public ICollection<Teacher> GetByTitleId(int titleId, UnitOfWork unitOfWork, bool pullCoursesSubjects = false)
        {
            var teachers = dbSet.Where(t => t.TitleId == titleId && t.IsActive).ToList();

            var css = context.Set<CoursesSubjects>()
                .Include(cs => cs.Subject)
                .Include(cs => cs.Course)
                .Include(cs => cs.Teachers)
                .ToList();

            teachers.ForEach(t => t.CourseSubjects = css.Where(cs => cs.Teachers.Any(tt => t.Id == tt.Id)).ToList());
            return teachers;
            //if (pullCoursesSubjects)
            //{
            //    var css = context.Set<CoursesSubjects>()

            //    //query = query.Include(t => t.CourseSubjects);
            //    //foreach (var q in query)
            //    //{
            //    //    foreach (var cs in q.CourseSubjects)
            //    //    {
            //    //        cs.Subject = unitOfWork.SubjectRepository.GetById(cs.SubjectId);
            //    //        cs.Course = unitOfWork.CourseRepository.GetById(cs.CourseId);
            //    //    }
            //    //}
            //}

            //return query.ToList();
        }
    }
}