using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class CategorySViewModel
    {
        [DisplayName("小分類編號")]
        public int CategorySID { get; set; }

        [DisplayName("小分類名稱")]
        [Required(ErrorMessage = "小分類名稱要輸入")]
        [MaxLength(20, ErrorMessage = "不能超過20個字")]
        public string CategorySName { get; set; }

        [DisplayName("中分類名稱")]
        public string CategoryMName { get; set; }

        [DisplayName("中分類編號")]
        public int CategoryMID { get; set; }
    }
}