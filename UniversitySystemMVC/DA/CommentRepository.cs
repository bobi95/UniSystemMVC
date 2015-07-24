using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class CommentRepository : BaseRepository<Comment>
    {
        public CommentRepository(AppContext context) : base(context) { }
    }
}