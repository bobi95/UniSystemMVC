using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Article : BaseEntity
    {
        public int SubjectId { get; set; }

        public int TeacherId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public virtual Subject Subject { get; set; }

        public virtual Teacher Teacher { get; set; }

        public virtual List<Comment> Comments { get; set; }

        public virtual List<Like> Likes { get; set; }
    }
}