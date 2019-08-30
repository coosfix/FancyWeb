using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{
    public class Totalrevenue
    {
        public List<string> year { get; set; }
        public List<totalGroup> totalGroup { get; set; }
        public Totalrevenue()
        {
            this.year = new List<string>();
            this.totalGroup = new List<totalGroup>();
        }
    }
    public class totalGroup
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<int> data { get; set; }
        public totalGroup()
        {
            this.type = "bar";
        }
    }

}