using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.Controllers
{
    public class CommentController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        //[HttpPost]
        //public ActionResult CreateComment(int? articleId)
        //{
        //    if (!articleId.HasValue)
        //    {
        //        return RedirectToAction("Index", "Article");
        //    }

        //    Article article = unitOfWork.ArticleRepository.GetById(articleId.Value);

        //    if (article == null)
        //    {
        //        return RedirectToAction("Index", "Article");
        //    }

        //    Comment comment = new Comment();
        //}

        public JsonResult CreateComment(string Title, string Content, int id, int UserId, UserTypeEnum UserType)
        {

            Comment comment = new Comment();
            comment.Title = Title;
            comment.Content = Content;
            comment.ArticleId = id;
            comment.DateCreated = DateTime.Now;
            comment.DateModified = DateTime.Now;
            comment.UserId = UserId;
            comment.UserType = UserType;

            unitOfWork.CommentRepository.Insert(comment);
            unitOfWork.Save();

            //return "Your comment was added. Refresh to see it, please :)";

            var newComment = new
            {
                Title = comment.Title,
                Content = comment.Content,
                DateCreated = comment.DateCreated
            };

            return Json(newComment, JsonRequestBehavior.AllowGet);
            //var subjects = unitOfWork.CoursesSubjectsRepository.GetByCourseId(id.Value).Select<CoursesSubjects, object>(cs =>
            //        new
            //        {
            //            Name = cs.Subject.Name,
            //            Id = cs.Subject.Id,
            //            Checked = t.CourseSubjects.Any(ts => ts.Id == cs.Id)
            //        });

            //return Json(subjects, JsonRequestBehavior.AllowGet);

            //if (!articleId.HasValue)
            //{
            //    return new EmptyResult();
            //}

            //Article a = unitOfWork.ArticleRepository.GetById(articleId.Value);

            //if (a == null)
            //{
            //    return new EmptyResult();
            //}

            //var subjects = unitOfWork.CoursesSubjectsRepository.GetByCourseId(id.Value).Select<CoursesSubjects, object>(cs =>
            //        new
            //        {
            //            Name = cs.Subject.Name,
            //            Id = cs.Subject.Id,
            //        });

            //return Json(subjects, JsonRequestBehavior.AllowGet);
        }
    }
}