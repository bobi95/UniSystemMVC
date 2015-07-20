using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.ViewModels.PagingVM
{
    public class PagingVM
    {
        public string Action { get; set; }

        public string Controller { get; set; }

        public string Prefix { get; set; }


        public int Page { get; set; }

        public int PerPage { get; set; }

        public int MaxPages { get; set; }

        public int PagesShown { get; set; }

        public Dictionary<string, object> LinkParameters { get; set; }

        public void CalcParameters(int count, int defaultPerPage, int pagesShown)
        {
            PagesShown = pagesShown;

            PerPage = PerPage > 0 ? PerPage : defaultPerPage;

            if (PerPage != defaultPerPage)
            {
                LinkParameters[Prefix + ".PerPage"] = PerPage;
            }

            MaxPages =
                count == 0 ? 1 : //if there are no results, show page 1
                count % PerPage == 0 ? count / PerPage : //if results fill pages entirely e.g. perpage = 10, results = 20 => pages = 2
                (count / PerPage) + 1; //if results don't fill pages entirely perpage = 10, results = 13 => pages = 2

            Page = MaxPages < Page ? MaxPages :
                Page < 1 ? 1 : Page;
        }
    }
}