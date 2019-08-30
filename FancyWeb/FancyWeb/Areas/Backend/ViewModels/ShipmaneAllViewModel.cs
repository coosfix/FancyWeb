using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ShipmaneAllViewModel
    {
        public int ShippingID { get; set; }
        public int OrderID { get; set; }
        public ShipHeaderViewModel shipheader;
        public IEnumerable<ShipDetailViewModel> shipdetailList;
    }
}