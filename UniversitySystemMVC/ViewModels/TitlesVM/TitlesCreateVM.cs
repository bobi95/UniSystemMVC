using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.ViewModels.TitlesVM
{
    public class TitlesCreateVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}