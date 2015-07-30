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
                Content = comment.Content
            };

            return Json(editedComment, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteComment(int commentId)
        {
            Comment comment = unitOfWork.CommentRepository.GetById(commentId);


            // Excecuted again .. 
            // TO BE FIXED

            if (comment == null)
            {
                return Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);
            }
            if (comment.Comments == null)
            {
                comment.Comments = new List<Comment>();     
            }
            

            foreach (var child in unitOfWork.CommentRepository.GetAll().Where(c=>c.CommentId == commentId))
            {
                unitOfWork.CommentRepository.Delete(child.Id);
            }
            unitOfWork.Save();

            comment.Comments.Clear(); 

            unitOfWork.CommentRepository.Delete(commentId);
            unitOfWork.Save();

            return Json(new object[]{new object()}, JsonRequestBehavior.AllowGet);
        }
    }
}