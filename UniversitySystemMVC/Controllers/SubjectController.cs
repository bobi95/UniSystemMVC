using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Filters;
using UniversitySystemMVC.ViewModels.SubjectsVM;
using UniversitySystemMVC.Extensions;

namespace UniversitySystemMVC.Controllers
{
    [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
    public class SubjectController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Subject
        public ActionResult Index()
        {
            return View();
        }

        #region CreateSubject
        [HttpGet]
        public ActionResult CreateSubject()
        {
            SubjectsCreateVM model = new SubjectsCreateVM();
            model.Courses = unitOfWork.CourseRepository.GetAll().ToList();

            return View("CreateEditSubject", model);
        }

        [HttpGet]
        public ActionResult EditSubject(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageSubjects", "Admin");
            }

            Subject subject = unitOfWork.SubjectRepository.GetById(id.Value);

            if (subject == null)
            {
                return RedirectToAction("ManageSubjects", "Admin");
            }

            SubjectsCreateVM model = new SubjectsCreateVM();
            model.Id = subject.Id;
            model.Name = subject.Name;
            model.Courses = unitOfWork.CourseRepository.GetAll().ToList();

            return View("CreateEditSubject", model);
        }

        [HttpPost]
        public ActionResult CreateEditSubject(SubjectsCreateVM model)
        {
            if (ModelState.IsValid)
            {
                Subject subject;
                if (model.Id == 0)
                {
                    subject = new Subject();
                }
                else
                {
                    subject = unitOfWork.SubjectRepository.GetById(model.Id);
                }

                subject.Name = model.Name;

                if (model.Id == 0)
                {
                    unitOfWork.SubjectRepository.Insert(subject);
                    TempData.FlashMessage("Subject has been created!");
                }
                else
                {
                    unitOfWork.SubjectRepository.Update(subject);
                    TempData.FlashMessage("Subject has been edited!");
                }
                unitOfWork.Save();


                model.Courses = unitOfWork.CourseRepository.GetAll().ToList();

                List<Course> courses = new List<Course>();

                foreach (var s in model.Courses)
                {
                    if ((Request.Form[s.Id.ToString()] != null) && (Request.Form[s.Id.ToString()] == "on"))
                    {
                        courses.Add(s);
                    }
                }

                unitOfWork.CoursesSubjectsRepository.UpdateTable(subject, courses);

                return RedirectToAction("ManageSubjects", "Admin");
            }

            return View(model);
        }
        #endregion CreateSubject

        #region DeleteSubject
        [HttpGet]
        public ActionResult DeleteSubject(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageSubjects", "Admin");
            }

            Subject subject = unitOfWork.SubjectRepository.GetById(id.Value);

            if (subject == null)
            {
                return RedirectToAction("ManageSubjects", "Admin");
            }

            SubjectsDeleteVM model = new SubjectsDeleteVM();
            model.Id = subject.Id;
            model.Name = subject.Name;

            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteSubject(SubjectsDeleteVM model)
        {
            if (ModelState.IsValid)
            {
                Subject subject = unitOfWork.SubjectRepository.GetById(model.Id, true);

                List<CoursesSubjects> cs = unitOfWork.CoursesSubjectsRepository.GetBySubjectId(subject.Id, true);

                cs.ForEach(x => x.Teachers.Clear());

                subject.CoursesSubjects.Clear();
                subject.Grades.Clear();

                unitOfWork.CoursesSubjectsRepository.UpdateTable(subject, new List<Course>());

                unitOfWork.SubjectRepository.Delete(subject.Id);

                unitOfWork.Save();

                TempData.FlashMessage("Subject has been deleted");
                return RedirectToAction("ManageSubjects", "Admin");
            }

            return View(model);
        }
        #endregion DeleteSubject

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageSubjects", "Admin");
            }

            Subject subject = unitOfWork.SubjectRepository.GetById(id.Value);

            if (subject == null)
            {
                return RedirectToAction("ManageSubjects", "Admin");
            }

            SubjectsDetailsVM model = new SubjectsDetailsVM();
            model.Subject = subject;
            model.CoursesSubjects = unitOfWork.CoursesSubjectsRepository.GetBySubjectId(subject.Id, true);

            return View(model);
        }
    }
}