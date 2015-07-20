using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Grade : BaseEntity
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public double GradeValue { get; set; }

        public virtual Student Student { get; set; }
        public virtual Subject Subject { get; set; }

        public override string ToString()
        {
            return this.GradeValue.ToString();
        }
    }

}