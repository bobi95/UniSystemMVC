using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.Controllers
{
    public class GradeController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Grade
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddGrade(int studentId = 1, int subjectId = 1, double gradeValue = 5.7)
        {
            Grade grade = new Grade();
            grade.GradeValue = gradeValue;
            grade.StudentId = studentId;
            grade.SubjectId = subjectId;

            unitOfWork.GradeRepository.Insert(grade);
            unitOfWork.Save();

            return RedirectToAction("Index", "Teacher");
        }
    }
}