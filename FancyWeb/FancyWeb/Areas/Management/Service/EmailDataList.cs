using FancyWeb.Areas.Management.ViewModels;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.Service
{
    public class EmailDataList
    {
        private FancyStoreEntities db = new FancyStoreEntities();

        public AllPrdlist GetALLProudct()
        {
            AllPrdlist data = new AllPrdlist();
            // 熱門
            var p = db.OrderDetails.GroupBy(n => n.ProductID.ToString()).OrderByDescending(n => n.Count())
                .Select(n => new { n.Key, item = n.ToList() }).ToList().Take(20);
            foreach (var item in p)
            {
                data.popu.Add(new allProduct
                {
                    pgid = item.Key,
                    pname = item.item.First().Product.ProductName
                });
            }
            //最新
            data.news = db.Products.OrderByDescending(m => m.ProductInDate).Select(m => new allProduct
            {
                pgid = m.ProductID.ToString(),
                pname = m.ProductName
            }).Take(20).ToList();
            //活動商品
            data.eva = db.ActivityProducts.OrderByDescending(e => e.Activity.ActivityName).Select(e => new allProduct
            {
                pgid = e.ProductID.ToString(),
                pname = e.Product.ProductName
            }).ToList();
            return data;
        }

    }
}