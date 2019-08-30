using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ProductWashingVIewModel
    {
        [DisplayName("編號")]
        public int ProductWashingID { get; set; }


        [DisplayName("洗滌方式")]
        [Required(ErrorMessage = "名稱要輸入")]
        public int WashingID { get; set; }

        [DisplayName("洗滌方式")]
        public string WashingName { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }

        [DisplayName("商品名稱")]
        public string ProductName { get; set; }

        [DisplayName("商品編號")]
        public int ProductID { get; set; }
    }
}