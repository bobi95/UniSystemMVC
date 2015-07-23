using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate = null, bool getActive = true)
        {
            if (getActive)
            {
                return base.GetAll(predicate).Where(u => u.IsActive);
            }

            return base.GetAll(predicate);
        }
    }
}