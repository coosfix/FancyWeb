using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.ViewModels
{
    public class OrderDetailView
    {
        public string ProductName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int? Freight { get; set; }
        public int UnitPrice { get; set; }
        public int QTY { get; set; }
        public int subtotal { get; set; }

    }
}