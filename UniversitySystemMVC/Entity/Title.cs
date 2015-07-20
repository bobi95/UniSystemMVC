using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Title : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}