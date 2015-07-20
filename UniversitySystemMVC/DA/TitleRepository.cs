using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class TitleRepository : BaseRepository<Title>
    {
        public TitleRepository(AppContext context) : base(context) { }

        public List<Title> GetAll(bool pullTeachers = false)
        {
            var query = dbSet.AsQueryable<Title>();
            if (pullTeachers)
            {
                query = query.Include(t => t.Teachers);
            }

            var result = query.ToList();
            result.ForEach(t =>
            {
                t.Teachers = t.Teachers.Where(x => x.IsActive).ToList();
            });

            return result;
        }

        public Title GetById(int id, bool pullTeachers = false)
        {
            var query = dbSet.Where(t => t.Id == id);

            if (pullTeachers)
            {
                query = query.Include(t => t.Teachers);
            }

            var result = query.FirstOrDefault();
            if (result != null && pullTeachers)
            {
                result.Teachers = result.Teachers.Where(t => t.IsActive).ToList();
            }

            return result;
        }
    }
}