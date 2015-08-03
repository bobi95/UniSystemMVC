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
    public class AccountController : BaseController
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        #region Login
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
        [ValidateAntiForgeryToken]
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
        #endregion EditProfile

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

        #region EditProfile
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
                //User user;
                ////UserRepository<User> repo;

                //switch (AuthenticationManager.UserType.Value)
                //{
                //    case UserTypeEnum.Administrator:
                //        AdministratorRepository adminrepo = unitOfWork.AdminRepository; 
                //        user = unitOfWork.AdminRepository.GetById(model.Id);
                //        break;
                //    case UserTypeEnum.Student:
                //        StudentRepository studentrepo = unitOfWork.StudentRepository; 
                //        user = unitOfWork.StudentRepository.GetById(model.Id);
                //        break;
                //    case UserTypeEnum.Teacher:
                //        TeacherRepository teacherrepo = unitOfWork.TeacherRepository; 
                //        user = unitOfWork.TeacherRepository.GetById(model.Id);
                //        break;
                //}
                //Type t = user.GetType();
                //if (t.IsAssignableFrom(typeof(User)))
                //{
                //    //UserRepository<t> repo = new UserRepository<t>;
                //}
              

                //if (PasswordHasher.Equals(model.OldPassword, user.Salt, user.Hash))
                //{
                //    user.Username = model.Username;
                //    user.Email = model.Email;
                //    AuthenticationManager.LoggedUser.Username = user.Username;

                //    if (model.NewPassword != null)
                //    {
                //        if (model.NewPassword.Length > 2)
                //        {
                //            var passPhrase = PasswordHasher.Hash(model.NewPassword);
                //            user.Hash = passPhrase.Hash;
                //            user.Salt = passPhrase.Salt;

                //            repo.Update(user);
                //            unitOfWork.Save();
                //            TempData.FlashMessage("You successfully updated your account!", null, FlashMessageTypeEnum.Green);
                //            return RedirectToAction("Index", "Home");
                //        }
                //        else
                //        {
                //            ModelState.AddModelError(String.Empty, "Password must be at least 3 symbols");
                //        }
                //    }
                //    else
                //    {
                //        repo.Update(user);
                //        unitOfWork.Save();
                //        TempData.FlashMessage("You successfully updated your account!", null, FlashMessageTypeEnum.Green);
                //        return RedirectToAction("Index", "Home");
                //    }
                //}
                //else
                //{
                //    ModelState.AddModelError(String.Empty, "Wrong Password!");
                //}

                switch (AuthenticationManager.UserType.Value)
                {
                    case UserTypeEnum.Administrator:

                        Administrator admin = unitOfWork.AdminRepository.GetById(model.Id);

                        if (PasswordHasher.Equals(model.OldPassword, admin.Salt, admin.Hash))
                        {

                            admin.Username = model.Username;
                            admin.Email = model.Email;
                            AuthenticationManager.LoggedUser.Username = admin.Username;

                            if (model.NewPassword != null)
                            {
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
                                    ModelState.AddModelError(String.Empty, "Password must be at least 3 symbols");
                                }
                            }
                            else
                            {
                                unitOfWork.AdminRepository.Update(admin);
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

                    case UserTypeEnum.Student:

                        Student student = unitOfWork.StudentRepository.GetById(model.Id);

                        if (PasswordHasher.Equals(model.OldPassword, student.Salt, student.Hash))
                        {

                            student.Username = model.Username;
                            student.Email = model.Email;
                            AuthenticationManager.LoggedUser.Username = student.Username;

                            if (model.NewPassword != null)
                            {
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
                                    ModelState.AddModelError(String.Empty, "Password must be at least 3 symbols");
                                }
                            }
                            else
                            {
                                unitOfWork.StudentRepository.Update(student);
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

                    case UserTypeEnum.Teacher:

                        Teacher teacher = unitOfWork.TeacherRepository.GetById(model.Id);

                        if (PasswordHasher.Equals(model.OldPassword, teacher.Salt, teacher.Hash))
                        {

                            teacher.Username = model.Username;
                            teacher.Email = model.Email;
                            AuthenticationManager.LoggedUser.Username = teacher.Username;

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
                                    ModelState.AddModelError(String.Empty, "Password must be at least 3 symbols");
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
        #endregion EditProfile
    }
}