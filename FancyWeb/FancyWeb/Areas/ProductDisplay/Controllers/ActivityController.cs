using FancyWeb.Areas.ProductDisplay.Models;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.ProductDisplay.Controllers
{
    public class ActivityController : Controller
    {
        public FancyStoreEntities db = new FancyStoreEntities();

        // GET: ProductDisplay/Activity
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Browse(int id = 0)
        {
            ViewBag.activityid = id;
            ViewBag.activityname = db.Activities.Find(id).ActivityName;
            TempData["activityoid"] = (Request.Cookies["activityoid"] == null) ? "1" : Request.Cookies["activityoid"].Values[0].ToString();

            return View();
        }

        public ActionResult GetProduct(int id = 0, int orderid = 1)
        {
            var preproducts = db.ProductColors.Where(p => p.Product.ActivityProducts.FirstOrDefault().Activity.ActivityID == id && p.Product.ActivityProducts.FirstOrDefault().Activity.ActivityName != null);

            var products = ProductMethod.CreateProductCells(preproducts);

            var orderresult = ProductMethod.SetCellsByOrder(products, orderid);

            return Json(orderresult.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}