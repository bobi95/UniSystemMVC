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
        [ValidateAntiForgeryToken]
        public ActionResult AddGrade(int studentId, int subjectId, double gradeValue)
        {
            Grade grade = new Grade();
            grade.GradeValue = gradeValue;
            grade.StudentId = studentId;
            grade.SubjectId = subjectId;

            unitOfWork.GradeRepository.Insert(grade);
            unitOfWork.Save();

            return RedirectToAction("Index", "Teacher");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGrade(int gradeId, double gradeValue)
        {
            Grade grade = unitOfWork.GradeRepository.GetById(gradeId);
            grade.GradeValue = gradeValue;
            unitOfWork.GradeRepository.Update(grade);
            unitOfWork.Save();

            return RedirectToAction("Index", "Teacher");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteGrade(int gradeId)
        {
            unitOfWork.GradeRepository.Delete(gradeId);
            unitOfWork.Save();

            return RedirectToAction("Index", "Teacher");
        }
    }
}