using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ProductStockVIewModel
    {
        [DisplayName("編號")]
        public int StockID { get; set; }

        [DisplayName("商品名稱")]
        public string ProductName { get; set; }

        [DisplayName("商品編號")]
        public int ProductID { get; set; }

        [DisplayName("顏色")]
        public string ProductColor { get; set; }

        [DisplayName("尺吋")]
        public string ProductSize { get; set; }

        [DisplayName("現有庫存量")]
        public int StockQTY { get; set; }

        [DisplayName("安全庫存量")]
        public int MinStock { get; set; }

        [DisplayName("圖片")]
        public Nullable<int> PhotoID { get; set; }

        public int ProductSizeID { get; set; }
        public int ProductColorID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}