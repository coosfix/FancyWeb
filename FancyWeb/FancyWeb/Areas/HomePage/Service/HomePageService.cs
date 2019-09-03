using FancyWeb.Areas.HomePage.ViewModels;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.HomePage.Service
{
    public class HomePageService
    {
        private FancyStoreEntities db = new FancyStoreEntities();
        public HomePageData HomePageData()
        {
            HomePageData data = new HomePageData();
            // 熱門
            var p = db.OrderDetails.GroupBy(n => n.ProductID.ToString()).OrderByDescending(n => n.Count())
                .Select(n => new { n.Key, item = n.ToList() }).ToList().Take(20);
            foreach (var item in p)
            {
                data.Popular.Add(new ProductsModels
                {
                    PID = item.Key,
                    PName = item.item.First().Product.ProductName,
                    CompanyName = item.item.First().Product.Supplier.SupplierName,
                    UnitPrice = item.item.First().UnitPrice
                });
            }
            //最新
            data.NEWs = db.Products.OrderByDescending(m => m.ProductInDate).Select(m => new ProductsModels
            {
                PID = m.ProductID.ToString(),
                PName = m.ProductName,
                CompanyName = m.Supplier.SupplierName,
                UnitPrice = m.UnitPrice
            }).Take(20).ToList();

            //隨機
            data.Ramdom = db.ProductEvaluations.OrderBy(n => Guid.NewGuid()).Select(n => new RamdomProducts
            {
                PID = n.ProductID.ToString(),
                PName = n.Product.ProductName,
                CompanyName = n.Product.Supplier.SupplierName,
                UnitPrice = n.Product.UnitPrice,
                Comment = n.Comment,
                Grade = n.Grade.ToString()
            }).Take(6).ToList();
            return data;
        }


        //我得最愛 +- 
        public bool AddSubFavorite(int uid, int pid)
        {
            var fapr = db.MyFavorites.Where(n => n.ProductID == pid && n.UserID == uid).FirstOrDefault();
            if (fapr == null)
            {
                db.MyFavorites.Add(new MyFavorite
                {
                    UserID = uid,
                    ProductID = pid
                });
                db.SaveChanges();
                return true;
            }
            else
            {
                db.MyFavorites.Remove(fapr);
                db.SaveChanges();
                return false;
            }
        }

        //是否為我的最愛
        public bool IsFavorite(string uid, string pid)
        {
            int uuid = Int32.Parse(uid);
            int ppid = Int32.Parse(pid);
            return db.MyFavorites.Any(n => n.UserID == uuid && n.ProductID == ppid);
        }

        public string[] IG_HashTagsLink(string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                tags[i] = db.Products.AsEnumerable().Where(n => n.ProductName.ToUpper().Contains(tags[i].ToUpper())).Select(n => n.ProductID.ToString()).FirstOrDefault() ?? "";
            }
            return tags;
        }
    }
}