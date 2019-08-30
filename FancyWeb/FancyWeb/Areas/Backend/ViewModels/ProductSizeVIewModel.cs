using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ProductSizeVIewModel
    {
        [DisplayName("編號")]
        public int ProductSizeID { get; set; }


        [DisplayName("尺吋大小")]
        [Required(ErrorMessage = "尺吋大小要輸入")]
        public int SizeID { get; set; }

        [DisplayName("尺吋大小")]
        public string SizeName { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }

        [DisplayName("商品名稱")]
        public string ProductName { get; set; }

        [DisplayName("商品編號")]
        public int ProductID { get; set; }
    }
}