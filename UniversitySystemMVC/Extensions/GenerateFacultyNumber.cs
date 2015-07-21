using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.Extensions
{
    public static class GenerateFacultyNumber
    {
        public static string Generate(Course course, UnitOfWork unitOfWork)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.Year % 100);
            sb.Append("01");
            sb.Append(course.Code);
            sb.Append("1");


            var students = unitOfWork.StudentRepository.GetAll().Where(s => s.FacultyNumber.StartsWith(sb.ToString()));

            int idNumber = students.Count() + 1;
            sb.Append(idNumber.ToString("000"));

            return sb.ToString();
        }
    }
}