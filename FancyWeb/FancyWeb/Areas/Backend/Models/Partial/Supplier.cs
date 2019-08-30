using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Models
{
    [MetadataType(typeof(SupplierMiddleMetadata))]
    public partial class Supplier
    {
        public class SupplierMiddleMetadata
        {
            [DisplayName("供應商編號")]
            public int SupplierID { get; set; }

            [DisplayName("供應商名稱")]
            [Required(ErrorMessage = "供應商名稱要輸入")]
            public string SupplierName { get; set; }

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