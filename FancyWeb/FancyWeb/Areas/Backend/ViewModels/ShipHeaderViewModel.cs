using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class ShipHeaderViewModel
    {
        [DisplayName("訂單編號")]
        public string OrderNum { get; set; }

        [DisplayName("訂單日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime OrderDate { get; set; }

        [DisplayName("付款方式")]
        public string PayMethodName { get; set; }

        [DisplayName("運送公司")]
        public string ShippingName { get; set; }

        [DisplayName("訂單狀態")]
        public string OrderStatusName { get; set; }

        [DisplayName("能否出貨")]
        public bool CanShip { get; set; }

        //隱藏欄位
        public int OrderID { get; set; }

        [DisplayName("運送公司")]
        public int ShippingID { get; set; }

        public int PayMethodID { get; set; }
        public int OrderStatusID { get; set; }
    }
}