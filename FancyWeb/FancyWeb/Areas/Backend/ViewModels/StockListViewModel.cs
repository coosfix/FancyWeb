using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class StockListViewModel
    {
        [DisplayName("商品名稱")]
        public string ProductName { get; set; }

        [DisplayName("顏色")]
        public string ProductColor { get; set; }

        [DisplayName("尺吋")]
        public string ProductSize { get; set; }

        [DisplayName("供應商")]
        public string SupplierName { get; set; }

        [DisplayName("最少補貨量")]
        public int AddQTY { get; set; }

        [DisplayName("現有庫存量")]
        public int StockQTY { get; set; }

        [DisplayName("已購未訂量")]
        public int CartQTY { get; set; }

        [DisplayName("已訂未出量")]
        public int OrderQTY { get; set; }

        [DisplayName("安全庫存量")]
        public int MinStock { get; set; }

        //以下為隱藏欄位
        public int StockID { get; set; }
        public int ProductID { get; set; }
        public int ProductSizeID { get; set; }
        public int ProductColorID { get; set; }
        public int PhotoID { get; set; }
    }
}