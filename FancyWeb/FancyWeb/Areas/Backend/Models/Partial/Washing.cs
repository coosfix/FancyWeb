using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Models
{
    [MetadataType(typeof(WashingMetadata))]
    public partial class Washing
    {
        public class WashingMetadata
        {
            [DisplayName("洗滌方式編號")]
            public int WashingID { get; set; }

            [DisplayName("洗滌方式名稱")]
            [Required(ErrorMessage = "洗滌方式名稱要輸入")]
            public string WashingName { get; set; }
        }
    }
}