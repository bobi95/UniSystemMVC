using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class AuthControllersRepository : BaseRepository<AuthController>
    {
        public AuthControllersRepository(AppContext context) : base(context) { }

        public void Save(AuthController item)
        {
            if (item.Id <= 0)
            {
                Insert(item);
            }
            else
            {
                Update(item);
            }
            base.Save();
        }
    }
}