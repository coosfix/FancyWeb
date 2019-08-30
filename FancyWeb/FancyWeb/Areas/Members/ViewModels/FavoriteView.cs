using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.ViewModels
{
    public class FavoriteView
    {
        public int FavoriteID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CompanyName { get; set; }
        public decimal UnitePrice { get; set; }
        public string Activity { get; set; }
        public decimal Discount{ get; set; }
        public DateTime? CreateDate { get; set; }
    }

}