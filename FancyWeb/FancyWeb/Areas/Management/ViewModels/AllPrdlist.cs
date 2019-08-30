using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{
    public class AllPrdlist
    {
        public List<allProduct> popu { get; set; }
        public List<allProduct> news{ get; set; }
        public List<allProduct> eva { get; set; }
        public AllPrdlist()
        {
            this.popu = new List<allProduct>();
            this.news = new List<allProduct>();
            this.eva = new List<allProduct>();
        }
    }

    public class allProduct
    {
        public string pgid { get; set; }
        public string pname { get; set; }
    }

}