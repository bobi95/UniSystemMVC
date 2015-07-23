using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Extensions;
using UniversitySystemMVC.Filters;
using UniversitySystemMVC.Models;
using UniversitySystemMVC.ViewModels.ArticlesVM;

namespace UniversitySystemMVC.Controllers
{
    public class ArticleController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Article
        public ActionResult Index()
        {
            ArticlesIndexVM model = new ArticlesIndexVM();
            model.Articles = unitOfWork.ArticleRepository.GetAll().ToList();

            return View(model);
        }

        #region CreateEditArticle

        private IEnumerable<SelectListItem> GetSubjects()
        {
            var subjects = unitOfWork.SubjectRepository.GetAll()
                        .Select(x =>
                                new SelectListItem
                                {
                                    Value = x.Id.ToString(),
                                    Text = x.Name
                                });

            return new SelectList(subjects, "Value", "Text");
        }

        [HttpGet]
        [AuthorizeUser(UserType = UserTypeEnum.Teacher, CheckType = true)]
        public ActionResult CreateArticle()
        {
            ArticlesCreateVM model = new ArticlesCreateVM();
            model.Subjects = GetSubjects();

            return View("CreateEditArticle", model);
        }

        [HttpGet]
        [AuthorizeUser(UserType = UserTypeEnum.Teacher, CheckType = true)]
        public ActionResult EditArticle(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Article");
            }

            Article article = unitOfWork.ArticleRepository.GetById(id.Value);

            if (article == null)
            {
                return RedirectToAction("Index", "Article");
            }

            ArticlesCreateVM model = new ArticlesCreateVM();
            model.Id = article.Id;
            model.Title = article.Title;
            model.Content = article.Content;
            model.DateCreated = article.DateCreated;
            model.Subjects = GetSubjects();

            return View("CreateEditArticle", model);
        }

        [HttpPost]
        [AuthorizeUser(UserType = UserTypeEnum.Teacher, CheckType = true)]
        public ActionResult CreateEditArticle(ArticlesCreateVM model)
        {
            if (ModelState.IsValid)
            {
                Article article;
                if (model.Id == 0)
                {
                    article = new Article();
                }
                else
                {
                    article = unitOfWork.ArticleRepository.GetById(model.Id);
                }

                article.Title = model.Title;
                article.Content = model.Content;
                
                article.Subject = unitOfWork.SubjectRepository.GetById(model.SubjectId);

                article.TeacherId = AuthenticationManager.LoggedUser.Id;

                if (model.Id == 0)
                {
                    article.DateCreated = DateTime.Now;
                    unitOfWork.ArticleRepository.Insert(article);
                    unitOfWork.Save();
                    TempData.FlashMessage("Article has been created!");
                }
                else
                {
                    article.DateCreated = model.DateCreated;
                    unitOfWork.ArticleRepository.Update(article);
                    unitOfWork.Save();
                    TempData.FlashMessage("Article has been edited!");
                }

                return RedirectToAction("Index");
            }

            model.Subjects = GetSubjects();
            return View(model);
        }

        #endregion
    }
}