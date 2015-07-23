using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Entity
{
    public class Like : BaseEntity
    {
        public int UserId { get; set; }

        public UserTypeEnum UserType { get; set; }

        public int ArticleId { get; set; }

        public Article Article { get; set; }
    }
}