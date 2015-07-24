using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.ArticlesVM
{
    public class ArticlesReadVM
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public List<Comment> Comments { get; set; }

        public User User { get; set; }
    }
}