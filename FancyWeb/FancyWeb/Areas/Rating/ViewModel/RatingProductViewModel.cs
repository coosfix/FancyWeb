using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Rating.ViewModel
{
    public class RatingProductViewModel
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int ProductColorID { get; set; }
        public int ProductSizeID { get; set; }
        public int UnitPrice { get; set; }
        public int OrderQTY { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> DiscountID { get; set; }


        public string OrderNum { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public Nullable<int> PhotoID { get; set; }
        //隱藏下面欄位
        public int UserID { get; set; }
    }
}