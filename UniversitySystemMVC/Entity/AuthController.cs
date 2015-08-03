using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class AuthController : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<AuthAction> AuthActions { get; set; }
    }
}