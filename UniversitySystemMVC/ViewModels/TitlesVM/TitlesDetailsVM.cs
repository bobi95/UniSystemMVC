using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.TitlesVM
{
    public class TitlesDetailsVM
    {
        public Title Title { get; set; }

        public List<Teacher> Teachers { get; set; }
    }
}