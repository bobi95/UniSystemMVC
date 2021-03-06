﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Filters;
using UniversitySystemMVC.ViewModels.TitlesVM;
using UniversitySystemMVC.Extensions;

namespace UniversitySystemMVC.Controllers
{
    [AuthorizeUser(UserType = UserTypeEnum.Administrator)]
    public class TitleController : BaseController
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        #region CreateTitle
        [HttpGet]
        public ActionResult CreateTitle()
        {
            return View("CreateEditTitle", new TitlesCreateVM());
        }

        [HttpGet]
        public ActionResult EditTitle(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageTitles", "Admin");
            }

            Title title = unitOfWork.TitleRepository.GetById(id.Value);

            if (title == null)
            {
                return RedirectToAction("ManageTitles", "Admin");
            }

            TitlesCreateVM model = new TitlesCreateVM();
            model.Id = title.Id;
            model.Name = title.Name;

            return View("CreateEditTitle", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEditTitle(TitlesCreateVM model)
        {
            if (ModelState.IsValid)
            {
                Title title;
                if (model.Id == 0)
                {
                    title = new Title();
                }
                else
                {
                    title = unitOfWork.TitleRepository.GetById(model.Id);
                }

                title.Name = model.Name;

                if (model.Id == 0)
                {
                    unitOfWork.TitleRepository.Insert(title);
                    TempData.FlashMessage("Title has been created!");
                }
                else
                {
                    unitOfWork.TitleRepository.Update(title);
                    TempData.FlashMessage("Title has been edited!");
                }
                unitOfWork.Save();

                return RedirectToAction("ManageTitles", "Admin");
            }

            return View(model);
        }
        #endregion CreateTitle

        #region DeleteTitle
        [HttpGet]
        public ActionResult DeleteTitle(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageTitles", "Admin");
            }

            Title title = unitOfWork.TitleRepository.GetById(id.Value);

            if (title == null)
            {
                return RedirectToAction("ManageTitles", "Admin");
            }

            TitlesDeleteVM model = new TitlesDeleteVM();
            model.Id = title.Id;
            model.Name = title.Name;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTitle(TitlesDeleteVM model)
        {
            if (ModelState.IsValid)
            {
                Title title = unitOfWork.TitleRepository.GetById(model.Id, true);

                if (title.Teachers != null || title.Teachers.Count > 0)
                {
                    TempData.FlashMessage("You cannot delete title that some teachers have!", null, FlashMessageTypeEnum.Red);
                    return View(model);    
                }

                unitOfWork.TitleRepository.Delete(title.Id);

                title.Teachers.Clear();

                unitOfWork.Save();

                TempData.FlashMessage("Title has been deleted!");
                return RedirectToAction("ManageTitles", "Admin");
            }

            return View(model);
        }
        #endregion DeleteTitle

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ManageTitles", "Admin");
            }

            Title title = unitOfWork.TitleRepository.GetById(id.Value);

            if (title == null)
            {
                return RedirectToAction("ManageTitles", "Admin");
            }

            TitlesDetailsVM model = new TitlesDetailsVM();
            model.Title = title;
            model.Teachers = unitOfWork.TeacherRepository.GetByTitleId(title.Id, unitOfWork, true).ToList();

            return View(model);
        }
    }
}