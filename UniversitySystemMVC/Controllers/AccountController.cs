using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Models;
using UniversitySystemMVC.ViewModels.UsersVM;
using UniversitySystemMVC.Extensions;

namespace UniversitySystemMVC.Controllers
{
    public class AccountController : Controller
    {
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
    }
}