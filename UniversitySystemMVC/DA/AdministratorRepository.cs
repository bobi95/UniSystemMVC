using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class AdministratorRepository : UserRepository<Administrator>
    {
        public AdministratorRepository(AppContext context) : base(context) { }
    }
}