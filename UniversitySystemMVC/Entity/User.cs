using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public abstract class User : BaseEntity
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Hash { get; set; }

        public string Salt { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsActive { get; set; }
    }

    public enum UserTypeEnum { Administrator, Student, Teacher }
}