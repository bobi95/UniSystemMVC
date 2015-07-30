using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Extensions;

namespace UniversitySystemMVC.Controllers
{
    public class GradeController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGrade(int studentId, int subjectId, string gradeValue)
        {
            try
            {
                Grade grade = new Grade();
                grade.GradeValue = double.Parse(gradeValue);
                grade.StudentId = studentId;
                grade.SubjectId = subjectId;

                if (grade.GradeValue < 2 || grade.GradeValue > 6)
                {
                    TempData.FlashMessage("Grade must be between 2.00 and 6.00!", null, FlashMessageTypeEnum.Red);
                    return RedirectToAction("Index", "Teacher");
                }

                unitOfWork.GradeRepository.Insert(grade);
                unitOfWork.Save();
            }
            catch (FormatException)
            {
                TempData.FlashMessage("Grade must be a number!", null, FlashMessageTypeEnum.Red);
            }

            return RedirectToAction("Index", "Teacher");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGrade(int gradeId, string gradeValue)
        {
            try
            {
                Grade grade = unitOfWork.GradeRepository.GetById(gradeId);
                grade.GradeValue = double.Parse(gradeValue);

                if (grade.GradeValue < 2 || grade.GradeValue > 6)
                {
                    TempData.FlashMessage("Grade must be between 2.00 and 6.00!", null, FlashMessageTypeEnum.Red);
                    return RedirectToAction("Index", "Teacher");
                }

                unitOfWork.GradeRepository.Update(grade);
                unitOfWork.Save();
            }
            catch (FormatException)
            {
                TempData.FlashMessage("Grade must be a number!", null, FlashMessageTypeEnum.Red);
            }
            

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