using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class GradeRepository : BaseRepository<Grade>
    {
        public GradeRepository(AppContext context) : base(context) { }

        public ICollection<Grade> GetByStudentId(int id)
        {
            return GetAll().Where(g => g.StudentId == id).ToList();
        }
    }
}