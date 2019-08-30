using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.ProductDisplay.Models
{
    [MetadataType(typeof(ProdcutMetaData))]
    public partial class Product
    {
        public class ProdcutMetaData
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public string Desctiption { get; set; }
            public int CategorySID { get; set; }
            [DisplayFormat(DataFormatString = "{0:c0}")]
            public int UnitPrice { get; set; }
            public Nullable<int> SupplierID { get; set; }
            public Nullable<System.DateTime> ProductInDate { get; set; }
            public Nullable<System.DateTime> ProductOutDate { get; set; }
            public Nullable<System.DateTime> CreateDate { get; set; }

        }
    }
}