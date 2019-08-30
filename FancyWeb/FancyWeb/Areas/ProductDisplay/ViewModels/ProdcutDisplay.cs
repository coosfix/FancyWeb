using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FancyWeb.Models;

namespace FancyWeb.Areas.ProductDisplay.ViewModels
{
    public class ProductDetail
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Desctiption { get; set; }
        public int CategorySID { get; set; }
        [DisplayFormat(DataFormatString = "{0:c0}")]
        public int UnitPrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:c0}")]
        public int SUnitPrice { get; set; }

        public string[] WashingName { get; set; }
        public List<Color> Colors { get; set; }
        public List<Size> Sizes { get; set; }

    }
}