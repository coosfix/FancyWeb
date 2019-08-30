using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.ProductDisplay.Models
{
    public class ProductCell
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategorySID { get; set; }
        public int ColorID { get; set; }
        public int? R { get; set; }
        public int? G { get; set; }
        public int? B { get; set; }
        public List<int> SizeID { get; set; }
        public int UnitPrice { get; set; }
        public decimal SUnitPrice { get; set; }
        public DateTime? ProductInDate { get; set; }
        public DateTime? ProductOutDate { get; set; }
        public int StockQTY { get; set; }
        public string ActivityName { get; set; }
    }
}