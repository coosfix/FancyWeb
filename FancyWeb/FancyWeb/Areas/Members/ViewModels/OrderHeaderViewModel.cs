using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.ViewModels
{
    public class OrderHeaderViewModel
    {
        public List<OrderHeaderView> orderList = new List<OrderHeaderView>();

        public Dictionary<string, int> DateCountCount()
        {
            var tt = this.orderList.GroupBy(n => n.OrderDate.Year + "/" + n.OrderDate.Month).Select(n => new
            {
                n.Key,
                datecount = n.Count()
            });
            Dictionary<string, int> keyValues = new Dictionary<string, int>();
            foreach (var item in tt)
            {
                keyValues.Add(item.Key, item.datecount);
            }
            return keyValues;
        }

        public Dictionary<string, int> StatusCount()
        {
            var tt = this.orderList.GroupBy(n => n.OrderStatus).Select(n => new
            {
                n.Key,
                datecount = n.Count()
            });
            Dictionary<string, int> keyValues = new Dictionary<string, int>();
            foreach (var item in tt)
            {
                keyValues.Add(item.Key, item.datecount);
            }
            return keyValues;
        }
    }
}