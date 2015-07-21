using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class CoursesSubjectsRepository : BaseRepository<CoursesSubjects>
    {
        public CoursesSubjectsRepository(AppContext context) : base(context) { }

        public List<CoursesSubjects> GetByCourseId(int id, bool pullTeachers = false)
        {
            var query = dbSet.Where(cs => cs.CourseId == id);
            if (pullTeachers)
            {
                query = query.Include(x => x.Teachers);
            }

            var result = query.Include(cs => cs.Subject).Include(cs => cs.Course).ToList();

            result.ForEach(cs => cs.Teachers = cs.Teachers.Where(t => t.IsActive).ToList());

            return result;
        }

        public List<CoursesSubjects> GetBySubjectId(int id, bool pullTeachers = false)
        {
            var query = dbSet.Where(cs => cs.SubjectId == id);
            if (pullTeachers)
            {
                query = query.Include(x => x.Teachers);
            }

            return query.Include(cs => cs.Subject).Include(cs => cs.Course).ToList();
        }

        public List<CoursesSubjects> GetStudentsDetails(int courseId, int studentId, UnitOfWork unitOfWork)
        {
            var css = dbSet.Where(cs => cs.CourseId == courseId).Include(cs => cs.Teachers).Include(cs => cs.Subject).ToList();
            var grades = unitOfWork.GradeRepository.GetByStudentId(studentId);

            css.ForEach(cs =>
            {
                cs.Teachers = cs.Teachers.Where(t => t.IsActive).ToList();
                cs.Subject.Grades = grades.Where(g => g.SubjectId == cs.Subject.Id).ToList();
            });

            return css;
        }

        public IEnumerable<CoursesSubjects> GetAll(bool pullTeachers = false)
        {
            var query = dbSet.AsQueryable<CoursesSubjects>();

            if (pullTeachers)
            {
                query = query.Include(cs => cs.Teachers);
            }

            return query.Include(cs => cs.Subject).Include(cs => cs.Course).ToList();
        }

        public IEnumerable<CoursesSubjects> GetAllByTeacherId(int teacherId)
        {
            return dbSet.Where(cs => cs.Teachers.Any(t => t.Id == teacherId))
                .Include(cs => cs.Teachers)
                .Include(cs => cs.Subject)
                .Include(cs => cs.Course)
                .ToList();

            //List<CoursesSubjects> result = new List<CoursesSubjects>();
            //foreach (var item in query)
            //{
            //    foreach (var t in item.Teachers)
            //    {
            //        if (t.Id == teacherId)
            //        {
            //            result.Add(item);
            //        }
            //    }
            //}

            //return css..ToList();

            //return result;
        }

        public void UpdateTable(Course course, IEnumerable<Subject> subjects)
        {
            var all = dbSet.Where(x => x.CourseId == course.Id).Include(x => x.Teachers).ToList();
            dbSet.RemoveRange(
                all.Where(r => subjects.FirstOrDefault(x => x.Id == r.SubjectId) == null)).Select(s =>
                    {
                        s.Teachers.Clear();
                        return s;
                    }
                );

            var newEntries = subjects.Where(x => all.FirstOrDefault(y => y.SubjectId == x.Id) == null);
            dbSet.AddRange(newEntries.Select<Subject, CoursesSubjects>(x => new CoursesSubjects() { CourseId = course.Id, SubjectId = x.Id }));

            context.SaveChanges();
        }

        public void UpdateTable(Subject subject, IEnumerable<Course> courses)
        {
            var all = dbSet.Where(x => x.SubjectId == subject.Id).ToList();
            dbSet.RemoveRange(all.Where(r => courses.FirstOrDefault(x => x.Id == r.CourseId) == null));

            var newEntries = courses.Where(x => all.FirstOrDefault(y => y.CourseId == x.Id) == null);
            dbSet.AddRange(newEntries.Select<Course, CoursesSubjects>(x => new CoursesSubjects() { SubjectId = subject.Id, CourseId = x.Id }));

            context.SaveChanges();
        }
    }
}