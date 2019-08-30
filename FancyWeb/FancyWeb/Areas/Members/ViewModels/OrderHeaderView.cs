using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.ViewModels
{
    public class OrderHeaderView
    {
        public int OrderID { get; set; }
        public string OrderNum { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public string PayMethod { get; set; }
        public string Shipping { get; set; }
        public string OrderStatus { get; set; }
        public decimal OrderAmount { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}