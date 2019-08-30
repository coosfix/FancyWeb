using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ProductPhotoVIewModel
    {
        [DisplayName("編號")]
        public int ProductPhotoID { get; set; }


        [DisplayName("圖片")]
        [Required(ErrorMessage = "圖片要輸入")]
        public int PhotoID { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }

        [DisplayName("商品名稱")]
        public string ProductName { get; set; }

        [DisplayName("商品編號")]
        public int ProductID { get; set; }

        [DisplayName("選擇圖檔")]
        public string PhotoFile { get; set; }
    }
}