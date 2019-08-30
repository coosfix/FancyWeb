using FancyWeb.Areas.HomePage.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.HomePage.Controllers
{
    public class HomeController : Controller
    {
        // GET: HomePage/Home
        HomePageService service = new HomePageService();
        public ActionResult Index()
        {
            return View(service.HomePageData());
        }

        public ActionResult FavoriteADDSub(int pid)
        {
            if (Request.Cookies.Get("upid") == null) return Json("Nope");

            int uid = Int32.Parse(Request.Cookies["upid"].Value);
            return service.AddSubFavorite(uid, pid) ? Json("Add") : Json("Sub");
        }
    }
}