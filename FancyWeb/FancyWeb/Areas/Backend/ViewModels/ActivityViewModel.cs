using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ActivityViewModel
    {
        [DisplayName("活動編號")]
        public int ActivityID { get; set; }

        [DisplayName("中分類名稱")]
        [Required(ErrorMessage = "中分類名稱必須要輸入")]
        [MaxLength(20, ErrorMessage = "不能超過100個字")]
        public string ActivityName { get; set; }

        [DisplayName("生效日期(起)")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> BeginDate { get; set; }

        [DisplayName("生效日期(迄)")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndDate { get; set; }


        [DisplayName("折扣編號")]
        public int DiscountID { get; set; }

        [DisplayName("折扣方式")]
        public string DiscountName { get; set; }

        [DisplayName("建立日期")]
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}