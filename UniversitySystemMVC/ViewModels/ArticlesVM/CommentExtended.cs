using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.ArticlesVM
{
    public class CommentExtended
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public int UserId { get; set; }

        public UserTypeEnum UserType { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public int? CommentId { get; set; }

        public Article Article { get; set; }

        public virtual Comment ParentComment { get; set; }

        public string FullName { get; set; }
    }
}