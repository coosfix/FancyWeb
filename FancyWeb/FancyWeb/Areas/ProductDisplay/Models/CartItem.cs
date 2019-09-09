using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.ProductDisplay.Models
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductColorID { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public int ProductSizeID { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int UnitPrice { get; set; }
        public int SUnitPrice { get; set; }
        public int OrderQTY { get; set; }
        public bool Enough { get; set; }
        public int DiscountID { get; set; }
        public string ActivityName { get; set; }
    }
}