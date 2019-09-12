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
            var activities = db.Activities.Where(a => a.EndDate >= DateTime.Now).ToList();
            return View(activities);
        }
        public ActionResult Browse(int id = 0)
        {
            if (id == 0)
                id = db.Activities.Where(a => a.EndDate >= DateTime.Now).FirstOrDefault().ActivityID;
            ViewBag.activityid = id;
            ViewBag.activityname = db.Activities.Find(id).ActivityName;
            TempData["activityoid"] = (Request.Cookies["activityoid"] == null) ? "1" : Request.Cookies["activityoid"].Values[0].ToString();
            int activitypicid = db.Activities.Find(id).PhotoID;

            ViewBag.activitypic = Convert.ToBase64String(db.Photos.Find(activitypicid).Photo1);

            return View();
        }

        public ActionResult GetProduct(int id = 0, int orderid = 1, int page = 1)
        {
            var preproducts = db.ProductColors.Where(p => p.Product.ActivityProducts.Where(a => a.Activity.EndDate >= DateTime.Now).FirstOrDefault().Activity.ActivityID == id && p.Product.ActivityProducts.Where(a => a.Activity.EndDate >= DateTime.Now).FirstOrDefault().Activity.ActivityName != null);

            var products = ProductMethod.CreateProductCells(preproducts);
            int pages = products.Count();
            var orderresult = ProductMethod.SetCellsByOrder(products, orderid).Skip((page - 1) * 16).Take(16);


            return Json(new { pages = pages, datas = orderresult.ToList() }, JsonRequestBehavior.AllowGet);
        }
    }
}