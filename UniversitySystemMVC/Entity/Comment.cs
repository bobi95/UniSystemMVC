using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Comment : BaseEntity
    {
        public int ArticleId  { get; set; }

        public int UserId { get; set; }

        public UserTypeEnum UserType { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public int? CommentId { get; set; }

        public virtual Article Article { get; set; }

        public virtual Comment ParentComment { get; set; }
    }
}