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
            sb.Append(course.Code.ToString("00"));
            sb.Append("1");

            int idNumber = 1;
            var students = unitOfWork.StudentRepository.GetAll(null, false);
            if (students.Count() > 0)
            {
                foreach (var s in students)
                {
                    if (s.FacultyNumber != null && s.FacultyNumber.StartsWith(sb.ToString()))
                    {
                        idNumber++;
                    }
                }
                students = students.Where(s => s.FacultyNumber.StartsWith(sb.ToString()));
            }
            
            sb.Append(idNumber.ToString("000"));

            return sb.ToString();
        }

        public static void ResetFacultyNumbersbyCourseId(int courseId, List<Student> students)
        {
            foreach (var s in students)
            {
                s.FacultyNumber = null;
            }
        }
    }
}