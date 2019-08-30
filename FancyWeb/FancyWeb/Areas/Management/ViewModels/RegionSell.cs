using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{
    public class RegionSell
    {
        public List<string> month { get; set; }
        
        public List<RegionGroup> regionGroup { get; set; }
        public RegionSell()
        {
            regionGroup = new List<RegionGroup>();
            month = new List<string>();
        }
    }

    public class RegionGroup
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<int> data { get; set; }
    }
}