using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Models
{
    [MetadataType(typeof(ShippingMiddleMetadata))]
    public partial class Shipping
    {
        public class ShippingMiddleMetadata
        {
            [DisplayName("運送公司編號")]
            public int ShippingID { get; set; }

            [DisplayName("運送公司名稱")]
            [Required(ErrorMessage = "運送公司名稱要輸入")]
            public string ShippingName { get; set; }

            [DisplayName("電話號碼")]
            public string Phone { get; set; }

            [DisplayName("傳真號碼")]
            public string Fax { get; set; }

            [DisplayName("電子郵件")]
            [EmailAddress(ErrorMessage = "Email格式不正確")]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [DisplayName("公司地址")]
            public string Address { get; set; }

            [DisplayName("建立日期")]
            public Nullable<System.DateTime> CreateDate { get; set; }
        }
    }
}