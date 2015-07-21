using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Filters;
using UniversitySystemMVC.ViewModels.CoursesVM;
using UniversitySystemMVC.Extensions;
using UniversitySystemMVC.ViewModels.StudentsVM;

namespace UniversitySystemMVC.Controllers
{
    [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
    public class CourseController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Course
        public ActionResult Index()
        {
            return View();
        }

        #region CreateCourse
        [HttpGet]
        public ActionResult CreateCourse()
        {
            CoursesCreateVM model = new CoursesCreateVM();
            model.Subjects = unitOfWork.SubjectRepository.GetAll(true);

            return View("CreateEditCourse", model);
        }

        [HttpGet]
        public ActionResult EditCourse(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageCourses", "Admin");
            }

            Course course = unitOfWork.CourseRepository.GetById(id.Value);

            if (course == null)
            {
                return RedirectToAction("ManageCourses", "Admin");
            }

            CoursesCreateVM model = new CoursesCreateVM();
            model.Id = course.Id;
            model.Name = course.Name;
            model.CourseCode = course.Code;
            model.Subjects = unitOfWork.SubjectRepository.GetAll(true);

            return View("CreateEditCourse", model);
        }

        [HttpPost]
        public ActionResult CreateEditCourse(CoursesCreateVM model)
        {
            if (ModelState.IsValid)
            {
                Course course;
                if (model.Id == 0)
                {
                    course = new Course();
                }
                else
                {
                    course = unitOfWork.CourseRepository.GetById(model.Id);
                }

                course.Name = model.Name;
                course.Code = model.CourseCode;

                model.Subjects = unitOfWork.SubjectRepository.GetAll();

                List<Subject> subjects = new List<Subject>();

                foreach (var s in model.Subjects)
                {
                    if ((Request.Form[s.Id.ToString()] != null) && (Request.Form[s.Id.ToString()] == "on"))
                    {
                        subjects.Add(s);
                    }
                }

                if (model.Id == 0)
                {
                    unitOfWork.CourseRepository.Insert(course);
                    TempData.FlashMessage("Course Created");
                }
                else
                {
                    unitOfWork.CourseRepository.Update(course);
                    TempData.FlashMessage("Course Edited");
                }
                unitOfWork.Save();

                unitOfWork.CoursesSubjectsRepository.UpdateTable(course, subjects);

                return RedirectToAction("ManageCourses", "Admin");
            }

            return View(model);
        }
        #endregion CreateCourse

        #region DeleteCourse
        [HttpGet]
        public ActionResult DeleteCourse(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageCourses", "Admin");
            }

            Course course = unitOfWork.CourseRepository.GetById(id.Value);

            if (course == null)
            {
                return RedirectToAction("ManageCourses", "Admin");
            }

            CoursesDeleteVM model = new CoursesDeleteVM();
            model.Id = course.Id;
            model.Name = course.Name;
            model.CourseCode = course.Code;

            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteCourse(CoursesDeleteVM model)
        {
            if (ModelState.IsValid)
            {
                Course course = unitOfWork.CourseRepository.GetById(model.Id);

                List<CoursesSubjects> cs = unitOfWork.CoursesSubjectsRepository.GetByCourseId(course.Id, true);

                cs.ForEach(x => x.Teachers.Clear());
                course.CoursesSubjects.Clear();
                course.Students.Clear();

                unitOfWork.CoursesSubjectsRepository.UpdateTable(course, new List<Subject>());

                unitOfWork.CourseRepository.Delete(course.Id);
                unitOfWork.Save();

                TempData.FlashMessage("Course Deleted");
                return RedirectToAction("ManageCourses", "Admin");
            }

            return View(model);
        }
        #endregion DeleteCourse

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageStudents", "Admin");
            }

            Course course = unitOfWork.CourseRepository.GetById(id.Value);

            if (course == null)
            {
                return RedirectToAction("ManageStudents", "Admin");
            }

            CoursesDetailsVM model = new CoursesDetailsVM();
            model.Course = course;
            model.Students = unitOfWork.StudentRepository.GetByCourseId(course.Id);

            return View(model);
        }
    }
}