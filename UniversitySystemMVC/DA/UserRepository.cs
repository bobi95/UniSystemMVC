using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public abstract class UserRepository<T> : BaseRepository<T> where T : User
    {
        public UserRepository(AppContext context) : base(context) { }

        public T GetByUsername(string username)
        {
            return GetAll().Where(u => u.Username == username).FirstOrDefault();
        }

        public override IEnumerable<T> GetAll()
        {
            return base.GetAll().Where(u => u.IsActive);
        }
    }
}