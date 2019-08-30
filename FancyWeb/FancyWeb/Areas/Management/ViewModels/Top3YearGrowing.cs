using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{
    public class Top3YearGrowing
    {
        public List<string> year { get; set; }
        public List<clsRevenueGroup>  clsRevenueGroup { get; set; }
    }
    public class clsRevenueGroup
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<int> data { get; set; }
    }
}