using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Models;

namespace UniversitySystemMVC.DA
{
    public class ArticleRepository : BaseRepository<Article>
    {
        public ArticleRepository(AppContext context) : base(context) { }

        public IEnumerable<Article> GetAll()
        {
            return context.Set<Article>().Where(a => a.Subject.CoursesSubjects.Any(cs => cs.Teachers.Any(t => t.Id == AuthenticationManager.LoggedUser.Id)) ||
                                                a.Subject.CoursesSubjects.Any(cs => cs.Course.Students.Any(s => s.Id == AuthenticationManager.LoggedUser.Id))).OrderBy(a => a.DateCreated).ToList();
        }
    }
}