
using FancyWeb.Areas.HomePage.Service;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.HomePage.ViewModels
{
    public class HomePageData
    {
        public List<ProductsModels> Popular { get; set; }
        public List<ProductsModels> NEWs { get; set; }
        public List<RamdomProducts> Ramdom { get; set; }

        public HomePageData()
        {
            this.Popular = new List<ProductsModels>();
            this.NEWs = new List<ProductsModels>();
            this.Ramdom = new List<RamdomProducts>();
        }

        HomePageService service = new HomePageService();
        public bool Favorstatus(string pid, string uid)
        {
            return service.IsFavorite(uid, pid);
        }

    }

    public class ProductsModels
    {
        public string PID { get; set; }
        public string PName { get; set; }
        public string CompanyName { get; set; }
        public int UnitPrice { get; set; }

    }
    public class RamdomProducts: ProductsModels
    {
        public string Comment { get; set; }
        public string Grade { get; set; }
    }
}