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
    public class LikeController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        public JsonResult AddLike(int articleId)
        {
            Like like = new Like();
            like.ArticleId = articleId;
            like.UserId = AuthenticationManager.LoggedUser.Id;
            like.UserType = AuthenticationManager.UserType.Value;
            like.DateCreated = DateTime.Now;

            if (unitOfWork.LikeRepository.GetAll().FirstOrDefault(l => l.ArticleId == articleId && l.UserType == AuthenticationManager.UserType && l.UserId == AuthenticationManager.LoggedUser.Id) == null)
            {
                unitOfWork.LikeRepository.Insert(like);
                unitOfWork.Save();
            }

            return Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteLike(int articleId)
        {
            Like like = unitOfWork.LikeRepository.GetAll().FirstOrDefault(l => l.ArticleId == articleId && l.UserType == AuthenticationManager.UserType && l.UserId == AuthenticationManager.LoggedUser.Id);
            unitOfWork.LikeRepository.Delete(like.Id);
            unitOfWork.Save();

            return Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);
        }
    }
}