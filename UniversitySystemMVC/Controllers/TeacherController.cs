﻿using System;
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
using UniversitySystemMVC.ViewModels.TeachersVM;
using UniversitySystemMVC.Extensions;
using UniversitySystemMVC.Models;
using System.Text;

namespace UniversitySystemMVC.Controllers
{
    public class TeacherController : BaseController
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Teacher
        [AuthorizeUser(UserType = UserTypeEnum.Teacher, CheckType = true)]
        public ActionResult Index()
        {
            Teacher teacher = unitOfWork.TeacherRepository.GetById(AuthenticationManager.LoggedUser.Id);

            if (teacher == null)
            {
                return RedirectToAction("Index", "Home");
            }

            TeachersIndexVM model = new TeachersIndexVM();
            model.CoursesSubjects = unitOfWork.CoursesSubjectsRepository.GetAllByTeacherId(teacher.Id).ToList();
            model.FullName = teacher.FirstName + " " + teacher.LastName;

            return View(model);
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
                return RedirectToAction("NotFound", "Error");
            }

            Teacher teacher = unitOfWork.TeacherRepository.GetById(id.Value);

            if (teacher == null || teacher.IsConfirmed)
            {
                return RedirectToAction("NotFound", "Error");
            }

            TeachersConfirmAccountVM model = new TeachersConfirmAccountVM();
            model.Id = id.Value;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmAccount(TeachersConfirmAccountVM model)
        {
            if (ModelState.IsValid)
            {
                Teacher teacher = unitOfWork.TeacherRepository.GetById(model.Id);
                teacher.Username = model.Username;

                if (teacher != null)
                {
                    if (PasswordHasher.Equals(model.Password, teacher.Salt, teacher.Hash))
                    {
                        var passPhrase = PasswordHasher.Hash(model.NewPassword);
                        teacher.Hash = passPhrase.Hash;
                        teacher.Salt = passPhrase.Salt;
                        teacher.IsConfirmed = true;
                        unitOfWork.TeacherRepository.Update(teacher);
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
        #endregion AccountConfirmation

        #region CreateTeacher
        [HttpGet]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        public ActionResult CreateTeacher()
        {
            TeachersCreateAccountVM model = new TeachersCreateAccountVM();
            model.UserType = UserTypeEnum.Teacher;
            model.Titles = GetTitles();
            model.Subjects = unitOfWork.SubjectRepository.GetAll();

            return View("CreateEditTeacher", model);
        }

        private IEnumerable<SelectListItem> GetTitles()
        {
            var titles = unitOfWork.TitleRepository.GetAll()
                        .Select(x =>
                                new SelectListItem
                                {
                                    Value = x.Id.ToString(),
                                    Text = x.Name
                                });

            return new SelectList(titles, "Value", "Text");
        }

        [HttpGet]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        public ActionResult EditTeacher(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            Teacher teacher = unitOfWork.TeacherRepository.GetById(id.Value);

            if (teacher == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            TeachersCreateAccountVM model = new TeachersCreateAccountVM();
            model.Id = teacher.Id;
            model.Username = teacher.Username;
            model.FirstName = teacher.FirstName;
            model.LastName = teacher.LastName;
            model.Email = teacher.Email;
            model.TitleId = teacher.TitleId ?? 0;
            model.Titles = GetTitles();
            model.Subjects = unitOfWork.SubjectRepository.GetAll();

            return View("CreateEditTeacher", model);
        }

        [HttpPost]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEditTeacher(TeachersCreateAccountVM model)
        {
            if (ModelState.IsValid)
            {
                Teacher teacher;

                if (model.Id == 0)
                {
                    teacher = new Teacher();
                }
                else
                {
                    teacher = unitOfWork.TeacherRepository.GetById(model.Id);
                }

                teacher.FirstName = model.FirstName;
                teacher.LastName = model.LastName;
                teacher.Email = model.Email;
                teacher.IsActive = true;


                teacher.Title = unitOfWork.TitleRepository.GetById(model.TitleId);
                if (teacher.Title == null)
                {
                    ModelState.AddModelError("TitleId", "No title");
                    model.Titles = GetTitles();
                    return View(model);
                }

                if (model.Id == 0)
                {
                    string password = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);

                    var passPhrase = PasswordHasher.Hash(password);

                    teacher.Hash = passPhrase.Hash;
                    teacher.Salt = passPhrase.Salt;
                    teacher.IsConfirmed = false;

                    unitOfWork.TeacherRepository.Insert(teacher);
                    unitOfWork.Save();
                    TempData.FlashMessage("Teacher has been created!");

                    #region Send password to mail
                    MailMessage message = new MailMessage();
                    message.IsBodyHtml = true;

                    message.Sender = new MailAddress("no-reply@unisystem.com");
                    message.To.Add(model.Email);
                    message.Subject = "Welcome to the University System";
                    message.From = new MailAddress("no-reply@unisystem.com");

                    StringBuilder msgBody = new StringBuilder();
                    msgBody.AppendLine(String.Format("<h3>Hello, {0} {1}</h3>", teacher.FirstName, teacher.LastName));
                    msgBody.AppendLine("<h4>Welcome to our University System!</h4>");
                    msgBody.AppendLine(String.Format("<p>You must confirm your account: <a href='{0}'>Confirm</a></p>", Url.Action("ConfirmAccount", "Teacher", new { id = teacher.Id }, Request.Url.Scheme)));
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
                    unitOfWork.TeacherRepository.Update(teacher);
                    unitOfWork.Save();
                    TempData.FlashMessage("Teacher has been edited!");
                }

                return RedirectToAction("ManageTeachers", "Admin");
            }

            model.Titles = GetTitles();
            return View(model);
        }
        #endregion CreateTeacher

        #region DeleteTeacher
        [HttpGet]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        public ActionResult DeleteTeacher(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageTeachers", "Admin");
            }

            Teacher teacher = unitOfWork.TeacherRepository.GetById(id.Value);

            if (teacher == null)
            {
                return RedirectToAction("ManageTeachers", "Admin");
            }

            TeachersDeleteAccountVM model = new TeachersDeleteAccountVM();
            model.Id = teacher.Id;
            model.Username = teacher.Username;
            model.FirstName = teacher.FirstName;
            model.LastName = teacher.LastName;
            model.Email = teacher.Email;
            model.Title = teacher.Title;

            return View(model);
        }

        [HttpPost]
        [AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTeacher(TeachersDeleteAccountVM model)
        {
            if (ModelState.IsValid)
            {
                Teacher teacher = unitOfWork.TeacherRepository.GetById(model.Id);
                teacher.IsActive = false;
                unitOfWork.Save();

                TempData.FlashMessage("Teacher has been deleted!");
                return RedirectToAction("ManageTeachers", "Admin");
            }

            return View(model);
        }
        #endregion DeleteTeacher

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageTeachers", "Admin");    
            }

            Teacher teacher = unitOfWork.TeacherRepository.GetById(id.Value);

            if (teacher == null)
            {
                return RedirectToAction("ManageTeachers", "Admin");
            }

            TeachersDetailsVM model = new TeachersDetailsVM();
            model.Teacher = teacher;
            model.CoursesSubjects = unitOfWork.CoursesSubjectsRepository.GetAllByTeacherId(teacher.Id).ToList();

            return View(model);
        }
    }
}