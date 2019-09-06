using FancyWeb.Areas.Management.ViewModels;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FancyWeb.Areas.Management.Service
{
    public class DashboardService
    {
        private FancyStoreEntities db = new FancyStoreEntities();

        //取得儀錶板部分資料
        public DashboardViewModel GetallData()
        {
            DashboardViewModel dashboard = new DashboardViewModel();
            dashboard.DaytotalReven = db.OrderHeaders.AsEnumerable().Where(n => n.CreateDate.Value.ToShortDateString() == DateTime.Now.ToShortDateString()).Sum(n => n.OrderAmount);
            dashboard.DayMembers = db.Users.AsEnumerable().Where(n => n.RegistrationDate.ToShortDateString() == DateTime.Now.ToShortDateString()).Count();
            dashboard.DayOrders = db.OrderHeaders.AsEnumerable().Where(n => n.CreateDate.Value.ToShortDateString() == DateTime.Now.ToShortDateString()).Count();
            dashboard.WaitShip = db.OrderHeaders.Count(n => n.OrderStatusID == 1);
            var ms = db.Users.GroupBy(n => n.OauthType).Select(n => new { n.Key, count = n.Count() }).ToDictionary(p => p.Key, p => p.count);
            if (!ms.ContainsKey("G")) ms.Add("G", 0);
            if (!ms.ContainsKey("F")) ms.Add("F", 0);
            if (!ms.ContainsKey("L")) ms.Add("L", 0);
            dashboard.MemberSource = ms;
            var gender = db.Users.GroupBy(n => n.RegistrationDate.Year).OrderByDescending(n => n.Key).Select(n => new
            {
                n.Key,
                f = n.Where(m => m.Gender).Count(),
                m = n.Where(m => !m.Gender).Count(),
            }).ToDictionary(p => p.Key.ToString(), p => new double[] { p.f, p.m });
            dashboard.MemberGender = gender;
            var pupp = db.OrderDetails.AsEnumerable().Where(n => n.CreateDate.Value.ToShortDateString() == DateTime.Now.ToShortDateString())
                       .GroupBy(n => n.ProductID).Select(n => new { n.Key, sum = n.Sum(m => m.OrderQTY) }).OrderByDescending(n => n.sum).Take(3).ToList();
            if (pupp.Count != 0)
            {
                foreach (var item in pupp)
                {
                    var product = db.Products.Find(item.Key);
                    dashboard.PopularProducts.Add(new PopularProducts
                    {
                        Pid = product.ProductID,
                        Pname = product.ProductName,
                        count = item.sum
                    });
                }
            }
            var rO = db.OrderHeaders.Where(n => n.OrderStatusID == 1).OrderByDescending(n => n.CreateDate).Take(5).ToList();
            dashboard.recentOrders = rO;
            var Ev = db.ProductEvaluations.OrderByDescending(n => n.EvaluationDate).Take(10);
            List<EvaluationViewModel> Evlist = new List<EvaluationViewModel>();
            foreach (var item in Ev)
            {
                var user = db.Users.Find(item.UserID);
                Evlist.Add(new EvaluationViewModel
                {
                    uphoto = db.Users.Find(item.UserID).OauthType != "N" ? Encoding.UTF8.GetString(user.Photo.Photo1) : $"data:Image/jpeg;base64,{Convert.ToBase64String(user.Photo.Photo1)}",
                    productname = item.Product.ProductName,
                    Username = db.Users.Find(item.UserID).UserName,
                    Uid = item.UserID,
                    Comment = item.Comment,
                    Date = ((DateTime)item.EvaluationDate).ToShortDateString(),
                    OrderNum = item.OrderNum
                });
            }
            dashboard.recentEvaluation = Evlist;

            return dashboard;
        }
        //總營收占比
        public Totalpercent Totalpercent()
        {
            Totalpercent totalpercent = new Totalpercent();
            var Prby = db.OrderDetails.Where(n => n.OrderHeader.OrderStatusID == 2);
            var clsPr = Prby.GroupBy(n => n.Product.CategorySmall.CategoryMiddle.CategoryMName)
                .Select(n => new
                {
                    n.Key,
                    total = n.Sum(s => s.UnitPrice * s.OrderQTY)
                }).ToList();
            var genderPr = Prby.GroupBy(n => (n.OrderHeader.User.Gender ? "男性" : "女性"))
                .Select(n => new
                {
                    n.Key,
                    total = n.Sum(s => s.UnitPrice * s.OrderQTY)
                }).ToDictionary(p => p.Key, p => p.total); ;
            List<Productspercent> productspercentlist = new List<Productspercent>();
            foreach (var item in clsPr)
            {
                productspercentlist.Add(new Productspercent
                {
                    name = item.Key,
                    value = item.total
                });
            }
            totalpercent.Productspercent = productspercentlist;
            totalpercent.Memberspercent = genderPr;
            return totalpercent;
        }
        //近三年類別銷售成長趨勢
        public Top3YearGrowing YearTop3growing()
        {
            var grbyY = db.OrderDetails.AsEnumerable().Where(n => n.OrderHeader.OrderStatusID == 2 &&
                n.OrderHeader.CreateDate.Value.Year >= DateTime.Now.AddYears(-2).Year)
                .GroupBy(n => n.OrderHeader.CreateDate.Value.Year).Select(n => new
                {
                    n.Key,
                    year = n.GroupBy(m => m.Product.CategorySmall.CategoryMiddle.CategoryMName)
                    .Select(m => new
                    {
                        m.Key,
                        cls = m.Sum(s => s.UnitPrice * s.OrderQTY)
                    }).ToList()
                }).ToList();
            Top3YearGrowing Year3Growing = new Top3YearGrowing();
            List<string> y = new List<string>();
            List<clsRevenueGroup> yy = new List<clsRevenueGroup>();
            foreach (var item in grbyY)
            {
                y.Add(item.Key.ToString());
                foreach (var item2 in item.year)
                {
                    if (yy.Any(n => n.name == item2.Key))
                    {
                        yy.Find(n => n.name == item2.Key).data.Add(item2.cls);
                    }
                    else
                    {
                        List<int> vs = new List<int>();
                        vs.Add(item2.cls);
                        yy.Add(new clsRevenueGroup
                        {
                            name = item2.Key,
                            type = "bar",
                            data = vs
                        });
                    }
                }
            }
            Year3Growing.year = y;
            Year3Growing.clsRevenueGroup = yy;
            return Year3Growing;
        }
        //地區銷售
        public RegionSell RegionSell()
        {
            var regionsell = db.OrderDetails.AsEnumerable().Where(n => n.OrderHeader.OrderStatusID == 2
            && n.OrderHeader.CreateDate.Value.Year == DateTime.Now.Year).GroupBy(n => n.CreateDate.Value.Month.ToString() + "月")
            .Select(n => new
            {
                n.Key,
                month = n.GroupBy(m => m.OrderHeader.User.Region.RegionName).Select(m => new
                {
                    m.Key,
                    region = m.Sum(s => s.UnitPrice * s.OrderQTY)
                }).ToList()
            }).ToList();
            RegionSell Rs = new RegionSell();
            for (int j = 0; j < regionsell.Count; j++)
            {
                Rs.month.Add(regionsell[j].Key);
                for (int i = 0; i < regionsell[j].month.Count; i++)
                {
                    if (Rs.regionGroup.Any(n => n.name == regionsell[j].month[i].Key))
                    {
                        Rs.regionGroup.Find(n => n.name == regionsell[j].month[i].Key).data[j] = regionsell[j].month[i].region;
                    }
                    else
                    {
                        List<int> rse = new List<int>(new int[regionsell.Count]);
                        rse[j] = regionsell[j].month[i].region;
                        Rs.regionGroup.Add(new RegionGroup
                        {
                            name = regionsell[j].month[i].Key,
                            type = "line",
                            data = rse
                        });
                    }
                }
            }
            return Rs;
        }

        //總營收
        public Totalrevenue Totalrevenue()
        {
            var totalreven = db.OrderDetails.AsEnumerable().Where(n => n.OrderHeader.OrderStatusID == 2 &&
                n.OrderHeader.CreateDate.Value.Year >= DateTime.Now.AddYears(-2).Year)
                .GroupBy(n => n.OrderHeader.CreateDate.Value.Year).Select(n => new
                {
                    n.Key,
                    year = n.GroupBy(m => seasons(Convert.ToDouble(m.CreateDate.Value.Month))).Select(m => new
                    {
                        m.Key,
                        season = m.Sum(s => s.UnitPrice * s.OrderQTY)
                    }).ToList()
                }).ToList();

            Totalrevenue totalrevenue = new Totalrevenue();
            foreach (var item in totalreven)
            {
                totalrevenue.year.Add(item.Key.ToString());
                foreach (var item2 in item.year)
                {
                    if (totalrevenue.totalGroup.Any(n => n.name == item2.Key))
                    {
                        totalrevenue.totalGroup.Find(n => n.name == item2.Key).data.Add(item2.season);
                    }
                    else
                    {
                        totalrevenue.totalGroup.Add(new totalGroup
                        {
                            name = item2.Key,
                            data = new List<int> { item2.season }
                        });
                    }
                }
            }
            return totalrevenue;
        }

        //全部訂單下單時間
        public string[] AllOrderDate(int? year)
        {
            string[] allo;
            if (year != 0)
            {
                allo = db.OrderHeaders.AsEnumerable().Where(n => n.CreateDate.Value.Year == year).Select(n => n.CreateDate.Value.ToString("yyyy/MM/dd")).ToArray();
            }
            else
            {
                allo = db.OrderHeaders.AsEnumerable().Select(n => n.CreateDate.Value.ToString("yyyy/MM/dd")).ToArray();
            }
            return allo;
        }

        public string[] YearJson()
        {
            return db.OrderHeaders.Select(n => n.CreateDate.Value.Year.ToString()).Distinct().ToArray();
        }
        public string seasons(double m)
        {
            return $"第{Convert.ToInt32(Math.Ceiling(m / 3))}季";
        }
    }
}