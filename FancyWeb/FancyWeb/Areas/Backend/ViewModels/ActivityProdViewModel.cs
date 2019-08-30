using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ActivityProdViewModel
    {
        [DisplayName("活動編號")]
        public int ActivityID { get; set; }

        [DisplayName("商品編號")]
        public int ProductID { get; set; }

        [DisplayName("商品名稱")]
        [Required(ErrorMessage = "商品名稱要輸入")]
        public string ProductName { get; set; }

        [DisplayName("分類")]
        public string CategoryMSName { get; set; }
        [DisplayName("供應商")]
        public string SupplierName { get; set; }

        [DisplayName("圖片")]
        public int PhotoID { get; set; }
    }
}