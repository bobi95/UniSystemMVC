using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class AuthAction : BaseEntity
    {
        public string Name { get; set; }

        public int AuthControllerId { get; set; }

        public virtual AuthController AuthController { get; set; }
    }
}