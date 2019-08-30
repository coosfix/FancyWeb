using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Models
{
    [MetadataType(typeof(SizeMetadata))]
    public partial class Size
    {
        public class SizeMetadata
        {
            [DisplayName("尺吋大小編號")]
            public int SizeID { get; set; }

            [DisplayName("尺吋大小名稱")]
            [Required(ErrorMessage = "尺吋大小名稱要輸入")]
            public string SizeName { get; set; }
        }
    }
}