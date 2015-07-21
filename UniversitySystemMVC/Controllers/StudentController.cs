using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Filters;
using UniversitySystemMVC.Hasher;
using UniversitySystemMVC.Models;
using UniversitySystemMVC.ViewModels.StudentsVM;
using UniversitySystemMVC.Extensions;

namespace UniversitySystemMVC.Controllers
{
    public class StudentController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Student
        [AuthorizeUser(UserType = UserTypeEnum.Student, CheckType = true)]
        public ActionResult Index()
        {
            if (AuthenticationManager.IsStudent)
            {
                StudentsIndexVM model = new StudentsIndexVM();
                model.Student = unitOfWork.StudentRepository.GetById(AuthenticationManager.LoggedUser.Id);
                model.Grades = unitOfWork.GradeRepository.GetByStudentId(AuthenticationManager.LoggedUser.Id).ToList();
                model.Course = unitOfWork.CourseRepository.GetById(((Student)AuthenticationManager.LoggedUser).CourseId.Value);
                model.CoursesSubjects = unitOfWork.CoursesSubjectsRepository.GetByCourseId(model.Course.Id, true);

                return View(model);
            }

            return RedirectToAction("NotAuthorized", "Error");
        }

        #region AccountConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmAccount(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            Student student = unitOfWork.StudentRepository.GetById(id.Value);

            if (student == null || student.IsConfirmed)
            {
                return RedirectToAction("Index", "Home");
            }

            StudentsConfirmAccountVM model = new StudentsConfirmAccountVM();
            model.Id = id.Value;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ConfirmAccount(StudentsConfirmAccountVM model)
        {
            if (ModelState.IsValid)
            {
                Student student = unitOfWork.StudentRepository.GetById(model.Id);
                student.Username = model.Username;

                if (student != null)
                {
                    if (PasswordHasher.Equals(model.Password, student.Salt, student.Hash))
                    {
                        student.IsConfirmed = true;
                        unitOfWork.StudentRepository.Update(student);
                        unitOfWork.Save();

                        TempData.FlashMessage("Your account has been confirmed. Please, login!");
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect Password");
                    }
                }
            }

            return View(model);
        }
        #endregion

        #region CreateStudent

        [HttpGet]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        public ActionResult CreateStudent()
        {
            StudentsCreateAccountVM model = new StudentsCreateAccountVM();
            model.UserType = UserTypeEnum.Student;
            model.Courses = GetCourses();

            return View("CreateEditStudent", model);
        }
        private IEnumerable<SelectListItem> GetCourses()
        {
            var courses = unitOfWork.CourseRepository.GetAll()
                        .Select(x =>
                                new SelectListItem
                                {
                                    Value = x.Id.ToString(),
                                    Text = x.Name,
                                    //Selected = x.Id==
                                });

            return new SelectList(courses, "Value", "Text");
        }

        [HttpGet]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        public ActionResult EditStudent(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageStudents", "Admin");
            }

            Student student = unitOfWork.StudentRepository.GetById(id.Value);

            if (student == null)
            {
                return RedirectToAction("ManageStudents", "Admin");
            }

            StudentsCreateAccountVM model = new StudentsCreateAccountVM();
            model.Id = student.Id;
            model.FirstName = student.FirstName;
            model.LastName = student.LastName;
            model.FacultyNumber = student.FacultyNumber;
            model.Email = student.Email;
            model.Courses = GetCourses();

            return View("CreateEditStudent", model);
        }

        [HttpPost]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        public ActionResult CreateEditStudent(StudentsCreateAccountVM model)
        {
            if (ModelState.IsValid)
            {
                Student student;

                if (model.Id == 0)
                {
                    student = new Student();
                }
                else
                {
                    student = unitOfWork.StudentRepository.GetById(model.Id);
                }

                student.FirstName = model.FirstName;
                student.LastName = model.LastName;
                student.Email = model.Email;
                //student.FacultyNumber = model.FacultyNumber;
                
                student.IsActive = true;

                if (model.Id == 0)
                {
                    string password = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);

                    var passPhrase = PasswordHasher.Hash(password);

                    student.Hash = passPhrase.Hash;
                    student.Salt = passPhrase.Salt;
                    student.IsConfirmed = false;
                    student.CourseId = model.CourseId;
                    Course course = unitOfWork.CourseRepository.GetById(model.CourseId);
                    student.FacultyNumber = GenerateFacultyNumber.Generate(course, unitOfWork);

                    unitOfWork.StudentRepository.Insert(student);
                    unitOfWork.Save();
                    TempData.FlashMessage("Student has been added. Faculty number: " + student.FacultyNumber);

                    #region Send password to mail
                    MailMessage message = new MailMessage();
                    message.Sender = new MailAddress("no-reply@unisystem.com");
                    message.To.Add(model.Email);
                    message.Subject = "Welcome to the University System";
                    message.From = new MailAddress("no-reply@unisystem.com");
                    message.Body = "Hello Student! Here is your password: " + password;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    #region Private
                    smtp.Credentials = new System.Net.NetworkCredential("phonebookadm@gmail.com", "programistaphonebook");
                    #endregion

                    smtp.Send(message);
                    #endregion
                }
                else
                {
                    if (student.CourseId != model.CourseId)
                    {
                        Course course = unitOfWork.CourseRepository.GetById(model.CourseId);
                        student.CourseId = course.Id;
                        student.FacultyNumber = GenerateFacultyNumber.Generate(course, unitOfWork);
                    }

                    unitOfWork.StudentRepository.Update(student);
                    unitOfWork.Save();
                    TempData.FlashMessage("Student has been edited. Faculty number: " + student.FacultyNumber);
                }

                return RedirectToAction("ManageStudents", "Admin");
            }

            return View(model);
        }

        #endregion CreateStudent

        #region DeleteStudent
        [HttpGet]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        public ActionResult DeleteStudent(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageStudents", "Admin");
            }

            Student student = unitOfWork.StudentRepository.GetById(id.Value);

            if (student == null)
            {
                return RedirectToAction("ManageStudents", "Admin");
            }

            StudentsDeleteAccountVM model = new StudentsDeleteAccountVM();
            model.Id = student.Id;
            model.Username = student.Username;
            model.FirstName = student.FirstName;
            model.LastName = student.LastName;
            model.Email = student.Email;
            model.FacultyNumber = student.FacultyNumber;

            return View(model);
        }

        [HttpPost]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        public ActionResult DeleteStudent(StudentsDeleteAccountVM model)
        {
            if (ModelState.IsValid)
            {
                Student student = unitOfWork.StudentRepository.GetById(model.Id);
                student.IsActive = false;
                unitOfWork.Save();

                TempData.FlashMessage("Student has been deactivated. Faculty number: " + student.FacultyNumber);
                return RedirectToAction("ManageStudents", "Admin");
            }

            return View(model);
        }
        #endregion DeleteStudent

        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageStudents", "Admin");
            }

            Student student = unitOfWork.StudentRepository.GetById(id.Value);

            if (student == null)
            {
                return RedirectToAction("ManageStudents", "Admin");
            }

            StudentDetailsVM model = new StudentDetailsVM();
            model.Id = student.Id;
            model.FirstName = student.FirstName;
            model.LastName = student.LastName;
            model.Username = student.Username;
            model.IsConfirmed = student.IsConfirmed;
            model.IsActive = student.IsActive;
            model.Email = student.Email;
            model.FacultyNumber = student.FacultyNumber;

            if (student.CourseId != null)
            {
                model.CoursesSubjects = unitOfWork.CoursesSubjectsRepository.GetStudentsDetails(student.CourseId.Value, student.Id, unitOfWork);
                model.Course = unitOfWork.CourseRepository.GetById(student.CourseId.Value);
            }

            return View(model);
        }
    }
}