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
            List<Subject> subjects = unitOfWork.SubjectRepository.GetAll(true)
                .Where(s => s.CoursesSubjects.Any(cs => cs.Teachers
                    .Any(t => t.Id == AuthenticationManager.LoggedUser.Id))).ToList();

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
        [ValidateAntiForgeryToken]
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

        [AuthorizeUser]
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
            model.User = article.Teacher;
            model.UserId = article.TeacherId;
            model.Likes = new List<LikeExtended>();

            foreach (var a in article.Likes)
            {
                LikeExtended likeExtended = new LikeExtended();
                likeExtended.Id = a.Id;
                likeExtended.UserId = a.UserId;
                likeExtended.UserType = a.UserType;
                likeExtended.ArticleId = a.ArticleId;
                likeExtended.DateCreated = a.DateCreated;

                Like currentLike = unitOfWork.LikeRepository.GetById(a.Id);
                switch (a.UserType)
                {
                    case UserTypeEnum.Administrator:
                        User admin = unitOfWork.AdminRepository.GetById(currentLike.UserId);
                        likeExtended.FullName = admin.FirstName + " " + admin.LastName;
                        break;
                    case UserTypeEnum.Student:
                        User student = unitOfWork.StudentRepository.GetById(currentLike.UserId);
                        likeExtended.FullName = student.FirstName + " " + student.LastName;
                        break;
                    case UserTypeEnum.Teacher:
                        User teacher = unitOfWork.TeacherRepository.GetById(currentLike.UserId);
                        likeExtended.FullName = teacher.FirstName + " " + teacher.LastName;
                        break;
                }

                model.Likes.Add(likeExtended);
            }

            model.LikeState = 0;
            if (article.Likes.FirstOrDefault(l => l.UserType == AuthenticationManager.UserType.Value && l.UserId == AuthenticationManager.LoggedUser.Id) != null)
            {
                model.LikeState = 1;
            }

            var comments = unitOfWork.CommentRepository.GetAll().Where(c => c.ArticleId == article.Id).ToList();
            model.Comments = new List<CommentExtended>();

            foreach (var c in comments)
            {
                CommentExtended commentExtended = new CommentExtended();
                commentExtended.Id = c.Id;;
                commentExtended.Title = c.Title;
                commentExtended.Content = c.Content;
                commentExtended.UserId = c.UserId;
                commentExtended.UserType = c.UserType;
                commentExtended.ArticleId = c.ArticleId;
                commentExtended.CommentId = c.CommentId;
                commentExtended.DateCreated = c.DateCreated;
                commentExtended.DateModified = c.DateModified;
                Comment currentComment = unitOfWork.CommentRepository.GetById(c.Id);
                switch (currentComment.UserType)
                {
                    case UserTypeEnum.Administrator:
                        User admin = unitOfWork.AdminRepository.GetById(currentComment.UserId);
                        commentExtended.FullName = admin.FirstName + " " + admin.LastName;
                        break;
                    case UserTypeEnum.Student:
                        User student = unitOfWork.StudentRepository.GetById(currentComment.UserId);
                        commentExtended.FullName = student.FirstName + " " + student.LastName;
                        break;
                    case UserTypeEnum.Teacher:
                        User teacher = unitOfWork.TeacherRepository.GetById(currentComment.UserId);
                        commentExtended.FullName = teacher.FirstName + " " + teacher.LastName;
                        break;
                }

                model.Comments.Add(commentExtended);
            }

            return View(model);
        }
    }
}