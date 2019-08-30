using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ShipDetailViewModel
    {
        [DisplayName("商品編號")]
        public int ProductID { get; set; }

        [DisplayName("商品名稱")]
        public string ProductName { get; set; }

        [DisplayName("顏色")]
        public string ProductColor { get; set; }

        [DisplayName("尺吋")]
        public string ProductSize { get; set; }

        [DisplayName("訂單數量")]
        public int OrderQTY { get; set; }

        [DisplayName("庫存量")]
        public int StockQTY { get; set; }

        [DisplayName("餘額")]
        public int BalanceQTY { get; set; }

        //以下為隱藏欄位
        public int StockID { get; set; }
        public int ProductSizeID { get; set; }
        public int ProductColorID { get; set; }
        [DisplayName("圖片")]
        public int PhotoID { get; set; }
    }
}