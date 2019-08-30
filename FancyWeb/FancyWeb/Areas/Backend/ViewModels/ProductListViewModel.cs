using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ProductListViewModel
    {
        [DisplayName("中分類")]
        public int CategoryMID { get; set; }

        [DisplayName("小分類")]
        public int CategorySID { get; set; }

        public IEnumerable<ProductViewModel> productlist;
    }
}