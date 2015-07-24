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
        [AuthorizeUser]
        public ActionResult Index()
        {
            ArticlesIndexVM model = new ArticlesIndexVM();
            List<Subject> subjects = GetSubjectsAsList().ToList();

            model.Articles = unitOfWork.ArticleRepository.GetAll().Where(a => subjects.Contains(a.Subject)).ToList();

            return View(model);
        }

        #region CreateEditArticle

        private ICollection<Subject> GetSubjectsAsList()
        {
            List<Subject> subjects = unitOfWork.SubjectRepository.GetAll(true).Where(s => s.CoursesSubjects.Any(cs => cs.Teachers.Any(t => t.Id == AuthenticationManager.LoggedUser.Id))).ToList();

            return subjects;
        } 
        private IEnumerable<SelectListItem> GetSubjects()
        {
            return GetSubjectsAsList().Select(s =>
                                new SelectListItem
                                {
                                    Value = s.Id.ToString(),
                                    Text = s.Name
                                });

            //var subjects = new List<Subject>();
            //foreach (var s in unitOfWork.SubjectRepository.GetAll())
            //{
            //    s.CoursesSubjects = unitOfWork.CoursesSubjectsRepository.GetBySubjectId(s.Id, true);
            //    foreach (var cs in s.CoursesSubjects)
            //    {
            //        if (s.Id == cs.Subject.Id)
            //        {
            //            foreach (var t in cs.Teachers)
            //            {
            //                if (t.Id == AuthenticationManager.LoggedUser.Id && !subjects.Contains(s))
            //                {
            //                    subjects.Add(s);
            //                }
            //            }    
            //        }

            //    }
            //}
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
                article.DateModified = DateTime.Now;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(UserType = UserTypeEnum.Teacher, CheckType = true)]
        public ActionResult DeleteArticle(int articleId)
        {
            unitOfWork.ArticleRepository.Delete(articleId);
            unitOfWork.Save();

            TempData.FlashMessage("The article was deleted!");
            return RedirectToAction("Index");
        }

        public ActionResult Read(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            Article article = unitOfWork.ArticleRepository.GetById(id.Value);

            if (article == null)
            {
                return RedirectToAction("Index");
            }

            ArticlesReadVM model = new ArticlesReadVM();
            model.Id = article.Id;
            model.SubjectId = article.SubjectId;
            model.Title = article.Title;
            model.Content = article.Content;
            model.DateCreated = article.DateCreated;
            model.DateModified = article.DateModified;
            model.Comments = unitOfWork.CommentRepository.GetAll().Where(c => c.ArticleId == article.Id).ToList();
            foreach (var c in model.Comments)
            {
                switch (c.UserType)
                {
                    case UserTypeEnum.Administrator:
                        model.User = unitOfWork.AdminRepository.GetById(c.UserId);
                        break;
                    case UserTypeEnum.Student:
                        model.User = unitOfWork.StudentRepository.GetById(c.UserId);
                        break;
                    case UserTypeEnum.Teacher:
                        model.User = unitOfWork.TeacherRepository.GetById(c.UserId);
                        break;
                }
            }

            return View(model);
        }
    }
}