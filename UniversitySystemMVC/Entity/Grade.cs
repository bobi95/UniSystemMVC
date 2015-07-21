using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Grade : BaseEntity, IComparable, IComparable<Grade>
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

        public int CompareTo(object obj)
        {
            var grade = obj as Grade;

            if (grade == null)
            {
                throw new ArgumentException();
            }

            return this.CompareTo(grade);
        }

        public int CompareTo(Grade other)
        {
            return this.GradeValue.CompareTo(other.GradeValue);
        }
    }

}