using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Models
{
    [MetadataType(typeof(ColorMetadata))]
    public partial class Color
    {
        public class ColorMetadata
        {
            [DisplayName("顏色編號")]
            public int ColorID { get; set; }

            [DisplayName("顏色名稱")]
            [Required(ErrorMessage = "顏色名稱要輸入")]
            public string ColorName { get; set; }

            [Required(ErrorMessage = "R要輸入")]
            public Nullable<int> R { get; set; }

            [Required(ErrorMessage = "G要輸入")]
            public Nullable<int> G { get; set; }

            [Required(ErrorMessage = "B要輸入")]
            public Nullable<int> B { get; set; }
        }
    }
}