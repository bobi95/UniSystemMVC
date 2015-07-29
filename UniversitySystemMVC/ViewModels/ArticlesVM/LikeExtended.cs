using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.ArticlesVM
{
    public class LikeExtended
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public UserTypeEnum UserType { get; set; }

        public int ArticleId { get; set; }

        public Article Article { get; set; }

        public string FullName { get; set; }

        public DateTime DateCreated { get; set; }
    }
}