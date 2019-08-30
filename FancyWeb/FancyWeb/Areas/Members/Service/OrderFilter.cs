using FancyWeb.Areas.Members.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.Service
{
    public class OrderFilter
    {
        public List<OrderHeaderView> list { get; set; }
        public string[] Statusfilter { get; set; }
        public string[] OrderDate { get; set; }
        public string orderby { get; set; }
    }
}