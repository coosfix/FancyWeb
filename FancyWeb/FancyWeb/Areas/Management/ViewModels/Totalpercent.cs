using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{

    public class Totalpercent
    {
        public List<Productspercent> Productspercent { get; set; }
        public Dictionary<string,int> Memberspercent { get; set; }
    }
    public class Productspercent
    {
        public string name { get; set; }
        public int value { get; set; }
    }

}