using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ProductViewModel
    {
        [DisplayName("商品編號")]
        public int ProductID { get; set; }

        [DisplayName("商品名稱")]
        [Required(ErrorMessage = "商品名稱要輸入")]
        public string ProductName { get; set; }

        [DisplayName("商品描述")]
        [MaxLength(200, ErrorMessage = "商品描述不能超過250字")]
        public string Desctiption { get; set; }

        [DisplayName("分類")]
        public int CategorySID { get; set; }

        [DisplayName("單價")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Range(1000, 10000, ErrorMessage = "商品價格在100 ~ 10000之間")]
        public int UnitPrice { get; set; }

        [DisplayName("供應商")]
        public Nullable<int> SupplierID { get; set; }

        [DisplayName("上架日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ProductInDate { get; set; }

        [DisplayName("下架日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ProductOutDate { get; set; }

        [DisplayName("建立日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CreateDate { get; set; }

        [DisplayName("分類")]
        public string CategoryMSName { get; set; }
        [DisplayName("供應商")]
        public string SupplierName { get; set; }

        [DisplayName("圖片")]
        public int PhotoID { get; set; }
    }
}