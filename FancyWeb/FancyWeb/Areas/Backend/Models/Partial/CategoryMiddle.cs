using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Models
{
    [MetadataType(typeof(CategoryMiddleMetadata))]
    public partial class CategoryMiddle
    {
        public class CategoryMiddleMetadata
        {
            [DisplayName("中分類編號")]
            public int CategoryMID { get; set; }

            [DisplayName("中分類名稱")]
            [Required(ErrorMessage = "中分類名稱要輸入")]
            [MaxLength(20, ErrorMessage = "不能超過20個字")]
            public string CategoryMName { get; set; }
        }
    }
}