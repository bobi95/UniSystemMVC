using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Models;

namespace UniversitySystemMVC.Controllers
{
    public class CommentController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        [HttpPost]
        public JsonResult CreateComment(int articleId, string title, string content, int? parentId)
        {

            Comment comment = new Comment();
            comment.Title = title;
            comment.Content = content;
            comment.ArticleId = articleId;
            comment.DateCreated = DateTime.Now;
            comment.DateModified = DateTime.Now;
            comment.UserId = AuthenticationManager.LoggedUser.Id;
            comment.UserType = AuthenticationManager.UserType.Value;
            if (parentId.HasValue)
            {
                comment.CommentId = parentId.Value;
            }

            string nameRes = String.Empty;
            switch (comment.UserType)
            {
                case UserTypeEnum.Administrator:
                    User admin = unitOfWork.AdminRepository.GetById(comment.UserId);
                    nameRes = admin.FirstName + " " + admin.LastName;
                    break;
                case UserTypeEnum.Student:
                    User student = unitOfWork.StudentRepository.GetById(comment.UserId);
                    nameRes = student.FirstName + " " + student.LastName;
                    break;
                case UserTypeEnum.Teacher:
                    User teacher = unitOfWork.TeacherRepository.GetById(comment.UserId);
                    nameRes = teacher.FirstName + " " + teacher.LastName;
                    break;
            }

            unitOfWork.CommentRepository.Insert(comment);
            unitOfWork.Save();

            var newComment = new
            {
                Id = comment.Id,
                Title = comment.Title,
                Content = comment.Content,
                DateCreated = comment.DateCreated,
                DateModified = comment.DateModified,
                Name = nameRes,
                ParentId = comment.CommentId
            };

            return Json(newComment, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditComment(int commentId, string title, string content)
        //, int id, int UserId, UserTypeEnum UserType
        {

            Comment comment = unitOfWork.CommentRepository.GetById(commentId);
            comment.Title = title;
            comment.Content = content;
            comment.DateModified = DateTime.Now;

            unitOfWork.CommentRepository.Update(comment);
            unitOfWork.Save();

            var editedComment = new
            {
                Title = comment.Title,
                Content = comment.Content,
                DateModified = comment.DateModified
            };

            return Json(editedComment, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteComment(int commentId)
        {
            unitOfWork.CommentRepository.Delete(commentId);
            unitOfWork.Save();

            // ?
            return Json(new object[]{new object()}, JsonRequestBehavior.AllowGet);
        }
    }
}