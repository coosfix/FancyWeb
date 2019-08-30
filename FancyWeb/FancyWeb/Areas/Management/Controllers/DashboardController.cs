using FancyWeb.Areas.Management.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.Management.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Management/Dashboard
        public ActionResult Index()
        {
            DashboardService service = new DashboardService();
            ViewBag.yearlist = service.YearJson();
            return View(service.GetallData());
        }
        //類別營收占比Json
        [HttpGet]
        public ActionResult Totalpercent()
        {
            DashboardService service = new DashboardService();
            return Json(service.Totalpercent(),JsonRequestBehavior.AllowGet);
        }

        //近三年類別銷售成長趨勢
        [HttpGet]
        public ActionResult YearTop3growing()
        {
            DashboardService service = new DashboardService();
            
            return Json(service.YearTop3growing(), JsonRequestBehavior.AllowGet);
        }

        //地區銷售
        [HttpGet]
        public ActionResult RegionSell()
        {
            DashboardService service = new DashboardService();

            return Json(service.RegionSell(), JsonRequestBehavior.AllowGet);
        }

        //總營收
        [HttpGet]
        public ActionResult Totalrevenue()
        {
            DashboardService service = new DashboardService();

            return Json(service.Totalrevenue(), JsonRequestBehavior.AllowGet);
        }
        //全部下單訂單日期
        [HttpGet]
        public ActionResult AllOrderDate(int? year)
        {
            DashboardService service = new DashboardService();
            return Json(service.AllOrderDate(year), JsonRequestBehavior.AllowGet);
        }
    }
}