using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class LikeRepository : BaseRepository<Like>
    {
        public LikeRepository(AppContext context) : base(context) { }
    }
}