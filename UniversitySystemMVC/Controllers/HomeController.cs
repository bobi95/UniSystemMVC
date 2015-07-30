using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.Models;

namespace UniversitySystemMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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

            return View();
        }
    }
}