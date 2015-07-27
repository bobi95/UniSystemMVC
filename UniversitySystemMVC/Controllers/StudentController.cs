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
using System.Text;

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
            if (AuthenticationManager.LoggedUser != null)
            {
                TempData.FlashMessage("You are logged in! Please log out and then verify!", null, FlashMessageTypeEnum.Red);
                return RedirectToAction("Index", "Home");
            }

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
                        var passPhrase = PasswordHasher.Hash(model.NewPassword);
                        student.Hash = passPhrase.Hash;
                        student.Salt = passPhrase.Salt;
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
                                    Text = x.Name
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
            model.CourseId = student.CourseId.Value;
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
                    message.IsBodyHtml = true;

                    message.Sender = new MailAddress("no-reply@unisystem.com");
                    message.To.Add(model.Email);
                    message.Subject = "Welcome to the University System";
                    message.From = new MailAddress("no-reply@unisystem.com");

                    StringBuilder msgBody = new StringBuilder();
                    msgBody.AppendLine(String.Format("<h3>Hello, {0} {1}</h3>", student.FirstName, student.LastName));
                    msgBody.AppendLine("<h4>Welcome to our University System!</h4>");
                    msgBody.AppendLine(String.Format("<p>You must confirm your account: <a href='{0}'>Confirm</a></p>", Url.Action("ConfirmAccount", "Student", new { id = student.Id}, Request.Url.Scheme)));
                    msgBody.AppendLine(String.Format("<p>Use this password to confirm: <strong>{0}</string></p>", password));
                    message.Body = msgBody.ToString();

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
        public ActionResult Details(int? id, StudentDetailsVM model, string submitBtn)
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

            model.Props = new Dictionary<string, object>();
            switch (model.SortOrder)
            {
                case "subject_desc":
                    model.CoursesSubjects = model.CoursesSubjects.OrderByDescending(cs => cs.Subject.Name).ToList();
                    break;
                case "subject_asc":
                default:
                    model.CoursesSubjects = model.CoursesSubjects.OrderBy(cs => cs.Subject.Name).ToList();
                    break;
            }

            if (submitBtn == "Export") // Export grades for single Student
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(String.Format("Subject,Grade"));
                foreach (var cs in model.CoursesSubjects)
                {
                    double total = 0.0;
                    double avg = 0.0;
                    if (cs.Subject != null)
                    {
                        
                        foreach (var grade in cs.Subject.Grades)
                        {
                            total += grade.GradeValue;
                        }
                        avg = total / cs.Subject.Grades.Count;
                    }
                    
                    sb.AppendLine(String.Format("{0},{1}", cs.Subject.Name, avg));
                }

                string filename = "students-grades-" + model.FacultyNumber + "-" + DateTime.Now.Date + ".csv"; 

                return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", filename);
            }

            return View(model);
        }
    }
}