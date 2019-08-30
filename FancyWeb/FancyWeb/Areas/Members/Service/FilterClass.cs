using FancyWeb.Areas.Members.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.Service
{
    public class FilterClass
    {
        public List<FavoriteView> list { get; set; }
        public string[] companyfilter { get; set; }
        public string[] activtyfilter { get; set; }
        public string Keyword { get; set; }
        public string orderby { get; set; }
    }
}