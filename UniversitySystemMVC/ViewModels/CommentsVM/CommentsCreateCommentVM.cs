using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.CommentsVM
{
    public class CommentsCreateCommentVM
    {
        public int ArticleId { get; set; }

        public int UserId { get; set; }

        public UserTypeEnum UserType { get; set; }

        public int Title { get; set; }

        public string Content { get; set; }
    }
}