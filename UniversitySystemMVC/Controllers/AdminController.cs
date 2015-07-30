using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Extensions;
using UniversitySystemMVC.Filters;
using UniversitySystemMVC.Hasher;
using UniversitySystemMVC.Models;
using UniversitySystemMVC.ViewModels.AdminsVM;
using UniversitySystemMVC.ViewModels.CoursesVM;
using UniversitySystemMVC.ViewModels.StudentsVM;
using UniversitySystemMVC.ViewModels.SubjectsVM;
using UniversitySystemMVC.ViewModels.TeachersVM;
using UniversitySystemMVC.ViewModels.TitlesVM;

namespace UniversitySystemMVC.Controllers
{
    [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
    public class AdminController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        #region Manages

        [HttpGet]
        public ActionResult ManageStudents()
        {
            AdminsManageStudentsVM model = new AdminsManageStudentsVM();
            model.Courses = unitOfWork.CourseRepository.GetAll().ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult ManageTeachers()
        {
            AdminsManageTeachersVM model = new AdminsManageTeachersVM();
            model.Teachers = unitOfWork.TeacherRepository.GetAll(t => t.IsActive).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult ManageAdmins()
        {
            AdminsManageAdminsVM model = new AdminsManageAdminsVM();
            model.Admins = unitOfWork.AdminRepository.GetAll(a => a.IsActive).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult ManageCourses()
        {
            AdminsManageCoursesVM model = new AdminsManageCoursesVM();
            model.Courses = unitOfWork.CourseRepository.GetAll().ToList();
            model.CoursesSubjects = (List<CoursesSubjects>)unitOfWork.CoursesSubjectsRepository.GetAll();
            return View(model);
        }

        [HttpGet]
        public ActionResult ManageSubjects()
        {
            AdminsManageSubjectsVM model = new AdminsManageSubjectsVM();
            model.Subjects = unitOfWork.SubjectRepository.GetAll(true).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult ManageTitles()
        {
            AdminsManageTitlesVM model = new AdminsManageTitlesVM();
            model.Titles = unitOfWork.TitleRepository.GetAll(true).ToList();

            return View(model);
        }

        #endregion Manages

        #region CreateAdmin
        [HttpGet]
        public ActionResult CreateAdmin()
        {
            AdminsCreateAccountVM model = new AdminsCreateAccountVM();
            model.UserType = UserTypeEnum.Administrator;

            return View("CreateEditAdmin", model);
        }

        public ActionResult EditAdmin(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageAdmins");
            }

            Administrator admin = unitOfWork.AdminRepository.GetById(id.Value);

            if (admin == null)
            {
                return RedirectToAction("ManageAdmins");
            }

            if (admin.Id != UniversitySystemMVC.Models.AuthenticationManager.LoggedUser.Id)
            {
                return RedirectToAction("ManageAdmins");
            }

            AdminsCreateAccountVM model = new AdminsCreateAccountVM();
            model.Id = admin.Id;
            model.FirstName = admin.FirstName;
            model.LastName = admin.LastName;
            model.Email = admin.Email;

            return View("CreateEditAdmin", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEditAdmin(AdminsCreateAccountVM model)
        {
            if (ModelState.IsValid)
            {
                Administrator admin;

                if (model.Id == 0)
                {
                    admin = new Administrator();
                }
                else
                {
                    admin = unitOfWork.AdminRepository.GetById(model.Id);
                }

                admin.FirstName = model.FirstName;
                admin.LastName = model.LastName;
                admin.Email = model.Email;
                admin.IsActive = true;

                if (model.Id == 0)
                {
                    string password = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);

                    var passPhrase = PasswordHasher.Hash(password);

                    admin.Hash = passPhrase.Hash;
                    admin.Salt = passPhrase.Salt;
                    admin.IsConfirmed = false;

                    unitOfWork.AdminRepository.Insert(admin);
                    unitOfWork.Save();

                    #region Send password to mail
                    MailMessage message = new MailMessage();
                    message.IsBodyHtml = true;

                    message.Sender = new MailAddress("no-reply@unisystem.com");
                    message.To.Add(model.Email);
                    message.Subject = "Welcome to the University System";
                    message.From = new MailAddress("no-reply@unisystem.com");

                    StringBuilder msgBody = new StringBuilder();
                    msgBody.AppendLine(String.Format("<h3>Hello, {0} {1}</h3>", admin.FirstName, admin.LastName));
                    msgBody.AppendLine("<h4>Welcome to our University System!</h4>");
                    msgBody.AppendLine(String.Format("<p>You must confirm your account: <a href='{0}'>Confirm</a></p>", Url.Action("ConfirmAccount", "Admin", new { id = admin.Id }, Request.Url.Scheme)));
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
                    unitOfWork.AdminRepository.Update(admin);
                    unitOfWork.Save();
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        #endregion CreateAdmin

        #region DeleteAdmin
        [HttpGet]
        public ActionResult DeleteAdmin(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageAdmin");
            }

            Administrator admin = unitOfWork.AdminRepository.GetById(id.Value);

            if (admin == null)
            {
                return RedirectToAction("ManageAdmin");
            }

            AdminsDeleteAdminVM model = new AdminsDeleteAdminVM();
            model.Id = admin.Id;
            model.Username = admin.Username;
            model.FirstName = admin.FirstName;
            model.LastName = admin.LastName;
            model.Email = admin.Email;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAdmin(AdminsDeleteAdminVM model)
        {
            if (ModelState.IsValid)
            {
                Administrator admin = unitOfWork.AdminRepository.GetById(model.Id);
                admin.IsActive = false;
                unitOfWork.Save();

                return RedirectToAction("ManageAdmins");
            }

            return View(model);
        }
        #endregion DeleteAdmin

        #region AdminAccountConfirmation
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
                return RedirectToAction("NotFound", "Error");
            }

            Administrator admin = unitOfWork.AdminRepository.GetById(id.Value);

            if (admin == null || admin.IsConfirmed)
            {
                return RedirectToAction("NotFound", "Error");
            }

            AdminsConfirmAccountVM model = new AdminsConfirmAccountVM();
            model.Id = id.Value;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmAccount(AdminsConfirmAccountVM model)
        {
            if (ModelState.IsValid)
            {
                Administrator admin = unitOfWork.AdminRepository.GetById(model.Id);
                admin.Username = model.Username;

                if (admin != null)
                {
                    if (PasswordHasher.Equals(model.Password, admin.Salt, admin.Hash))
                    {
                        var passPhrase = PasswordHasher.Hash(model.NewPassword);
                        admin.Hash = passPhrase.Hash;
                        admin.Salt = passPhrase.Salt;
                        admin.IsConfirmed = true;
                        unitOfWork.AdminRepository.Update(admin);
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
        #endregion AdminAccountConfirmation

        #region AssignTeacher
        [HttpGet]
        public ActionResult AssignTeacher(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageTeachers");
            }

            TeachersAssignVM model = new TeachersAssignVM();
            model.Id = id.Value;
            model.Courses = unitOfWork.CourseRepository.GetAll().ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTeacher(TeachersAssignVM model)
        {
            if (ModelState.IsValid)
            {
                Course selectedCourse = unitOfWork.CourseRepository.GetById(model.SelectedCourseId);

                Teacher teacher = unitOfWork.TeacherRepository.GetById(model.Id, true);

                List<CoursesSubjects> css = unitOfWork.CoursesSubjectsRepository.GetByCourseId(model.SelectedCourseId);

                int a;
                // keys to be assigned for the course
                List<int> ids = Request.Form.AllKeys.Where(x =>
                    int.TryParse(x, out a)).Select<string, int>(x => int.Parse(x)).ToList();

                List<int> newIds = new List<int>(ids);

                teacher.CourseSubjects.RemoveAll(x =>
                {
                    if(x.CourseId != selectedCourse.Id)
                    {
                        return false;
                    }

                    if (ids.Contains(x.Id))
                    {
                        newIds.Remove(x.Id);
                        return false;
                    }
                    return true;
                });

                teacher.CourseSubjects.AddRange(css.Where(cs =>
                    cs.CourseId == selectedCourse.Id &&
                    newIds.Contains(cs.SubjectId)));

                unitOfWork.Save();

                return RedirectToAction("ManageTeachers");
            }

            model.Courses = unitOfWork.CourseRepository.GetAll().ToList();
            return View(model);
        }
        #endregion AssignTeacher

        [HttpGet]
        public ActionResult GetSubjects(int? id, int? teacher)
        {
            if (!id.HasValue || !teacher.HasValue)
            {
                return new EmptyResult();
            }

            Teacher t = unitOfWork.TeacherRepository.GetById(teacher.Value, true);

            if (t == null)
            {
                return new EmptyResult();
            }

            var subjects = unitOfWork.CoursesSubjectsRepository.GetByCourseId(id.Value).Select<CoursesSubjects, object>(cs =>
                    new
                    {
                        Name = cs.Subject.Name,
                        Id = cs.Subject.Id,
                        Checked = t.CourseSubjects.Any(ts => ts.Id == cs.Id)
                    });

            return Json(subjects, JsonRequestBehavior.AllowGet);
        }
    }
}