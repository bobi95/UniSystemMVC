using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Models;
using UniversitySystemMVC.ViewModels.UsersVM;
using UniversitySystemMVC.Extensions;
using UniversitySystemMVC.ViewModels.AccountsVM;
using UniversitySystemMVC.Filters;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Hasher;

namespace UniversitySystemMVC.Controllers
{
    public class AccountController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (AuthenticationManager.LoggedUser == null)
            {
                return View(new UsersLoginVM());
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(UsersLoginVM model)
        {
            if (ModelState.IsValid)
            {
                AuthenticationManager.AuthenticateUser(model.Username, model.Password, model.UserType);
                if (AuthenticationManager.LoggedUser != null && AuthenticationManager.LoggedUser.IsActive)
                {
                    TempData.FlashMessage("", "Welcome, " + AuthenticationManager.LoggedUser.Username, FlashMessageTypeEnum.Green);

                    if (AuthenticationManager.IsAdmin)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (AuthenticationManager.IsStudent)
                    {
                        return RedirectToAction("Index", "Student");
                    }
                    else if (AuthenticationManager.IsTeacher)
                    {
                        return RedirectToAction("Index", "Teacher");
                    }

                }
                ModelState.AddModelError("", "Invalid data! Try again!");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (AuthenticationManager.LoggedUser != null)
            {
                TempData.FlashMessage("Successfully Logged-Out", "Goodbye, " + AuthenticationManager.LoggedUser.Username, FlashMessageTypeEnum.Green);
                AuthenticationManager.Logout();
            }

            return RedirectToAction("Index", "Home");
        }

        [AuthorizeUser]
        public ActionResult EditProfile()
        {
            AccountsEditProfileVM model = new AccountsEditProfileVM();

            switch (AuthenticationManager.UserType.Value)
            {
                case UserTypeEnum.Administrator:

                    Administrator admin = unitOfWork.AdminRepository.GetById(AuthenticationManager.LoggedUser.Id);
                    if (admin == null)
                    {
                        TempData.FlashMessage("User with this id cannot be found", null, FlashMessageTypeEnum.Red);
                        return RedirectToAction("Index", "Home");
                    }

                    model = new AccountsEditProfileVM();
                    model.Id = admin.Id;
                    model.Username = admin.Username;
                    model.Email = admin.Email;
                    break;

                case UserTypeEnum.Student:

                    Student student = unitOfWork.StudentRepository.GetById(AuthenticationManager.LoggedUser.Id);
                    if (student == null)
                    {
                        TempData.FlashMessage("User with this id cannot be found", null, FlashMessageTypeEnum.Red);
                        return RedirectToAction("Index", "Home");
                    }

                    model.Id = student.Id;
                    model.Username = student.Username;
                    model.Email = student.Email;
                    break;

                case UserTypeEnum.Teacher:
                    Teacher teacher = unitOfWork.TeacherRepository.GetById(AuthenticationManager.LoggedUser.Id);
                    if (teacher == null)
                    {
                        TempData.FlashMessage("User with this id cannot be found", null, FlashMessageTypeEnum.Red);
                        return RedirectToAction("Index", "Home");
                    }

                    model.Id = teacher.Id;
                    model.Username = teacher.Username;
                    model.Email = teacher.Email;
                    break;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(AccountsEditProfileVM model)
        {
            if (ModelState.IsValid)
            {
                switch (AuthenticationManager.UserType.Value)
                {
                    case UserTypeEnum.Administrator:

                        Administrator admin = unitOfWork.AdminRepository.GetById(model.Id);

                        if (PasswordHasher.Equals(model.OldPassword, admin.Salt, admin.Hash))
                        {
                            admin.Username = model.Username;
                            admin.Email = model.Email;

                            if (model.NewPassword.Length > 2)
                            {
                                var passPhrase = PasswordHasher.Hash(model.NewPassword);
                                admin.Hash = passPhrase.Hash;
                                admin.Salt = passPhrase.Salt;

                                unitOfWork.AdminRepository.Update(admin);
                                unitOfWork.Save();
                                TempData.FlashMessage("You successfully updated your account!", null, FlashMessageTypeEnum.Green);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ModelState.AddModelError(String.Empty, "Password must be at least 2 symbols");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "Wrong Password!");
                        }
                        
                        break;

                    case UserTypeEnum.Student:

                        Student student = unitOfWork.StudentRepository.GetById(model.Id);

                        if (PasswordHasher.Equals(model.OldPassword, student.Salt, student.Hash))
                        {

                            student.Username = model.Username;
                            student.Email = model.Email;

                            if (model.NewPassword.Length > 2)
                            {
                                var passPhrase = PasswordHasher.Hash(model.NewPassword);
                                student.Hash = passPhrase.Hash;
                                student.Salt = passPhrase.Salt;

                                unitOfWork.StudentRepository.Update(student);
                                unitOfWork.Save();
                                TempData.FlashMessage("You successfully updated your account!", null, FlashMessageTypeEnum.Green);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ModelState.AddModelError(String.Empty, "Password must be at least 2 symbols");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "Wrong Password!");
                        }

                        break;

                    case UserTypeEnum.Teacher:

                        Teacher teacher = unitOfWork.TeacherRepository.GetById(model.Id);

                        if (PasswordHasher.Equals(model.OldPassword, teacher.Salt, teacher.Hash))
                        {

                            teacher.Username = model.Username;
                            teacher.Email = model.Email;

                            if (model.NewPassword != null)
                            {
                                if (model.NewPassword.Length > 2)
                                {
                                    var passPhrase = PasswordHasher.Hash(model.NewPassword);
                                    teacher.Hash = passPhrase.Hash;
                                    teacher.Salt = passPhrase.Salt;

                                    unitOfWork.TeacherRepository.Update(teacher);
                                    unitOfWork.Save();
                                    TempData.FlashMessage("You successfully updated your account!", null, FlashMessageTypeEnum.Green);
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    ModelState.AddModelError(String.Empty, "Password must be at least 2 symbols");
                                }
                            }
                            else
                            {
                                unitOfWork.TeacherRepository.Update(teacher);
                                unitOfWork.Save();
                                TempData.FlashMessage("You successfully updated your account!", null, FlashMessageTypeEnum.Green);
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "Wrong Password!");
                        }

                        break;
                }
            }

            return View(model);
        }
    }
}