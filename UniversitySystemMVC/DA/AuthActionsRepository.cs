using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class AuthActionsRepository : BaseRepository<AuthAction>
    {
        public AuthActionsRepository(AppContext context) : base(context) { }

        public void Save(AuthAction item)
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