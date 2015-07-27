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

        [HttpPost]
        public JsonResult CreateComment(int id, string title, string content, int userId, UserTypeEnum userType)
        //, int id, int UserId, UserTypeEnum UserType
        {

            Comment comment = new Comment();
            comment.Title = title;
            comment.Content = content;
            comment.ArticleId = id;
            comment.DateCreated = DateTime.Now;
            comment.DateModified = DateTime.Now;
            comment.UserId = userId;
            comment.UserType = userType;

            string name =  String.Empty;
            switch (userType)
            {
                case UserTypeEnum.Administrator:
                    User admin =  unitOfWork.AdminRepository.GetById(comment.UserId);
                    name = admin.FirstName + " " + admin.LastName;
                    break;
                case UserTypeEnum.Student:
                    User student = unitOfWork.StudentRepository.GetById(comment.UserId);
                    name = student.FirstName + " " + student.LastName;
                    break;
                case UserTypeEnum.Teacher:
                    User teacher = unitOfWork.TeacherRepository.GetById(comment.UserId);
                    name = teacher.FirstName + " " + teacher.LastName;
                    break;
            }

            unitOfWork.CommentRepository.Insert(comment);
            unitOfWork.Save();

            //return "Your comment was added. Refresh to see it, please :)";

            var newComment = new
            {
                Id = comment.Id,
                Title = comment.Title,
                Content = comment.Content,
                DateCreated = comment.DateCreated,
                DateModified = comment.DateModified,
                Name = name
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