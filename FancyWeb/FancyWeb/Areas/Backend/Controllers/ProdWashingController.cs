using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using FancyWeb.Areas.Backend.ViewModels;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class ProdWashingController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/ProdWashing
        public ActionResult Index(int id)
        {
            ProductWashingVIewModel pwvm = new ProductWashingVIewModel();

            var pd = db.Products.Find(id);

            pwvm.ProductID = pd.ProductID;
            pwvm.ProductName = pd.ProductName;
            ViewBag.productwashing = db.Washings;

            return View(pwvm);
        }
               
        public ActionResult ProdWashingJson(int id)
        {
            List<ProductWashingVIewModel> pwvmlist = new List<ProductWashingVIewModel>();

            var pw = db.ProductWashings.Where(x => x.ProductID == id).OrderByDescending(x => x.ProductWashingID);
            var ws = db.Washings;

            foreach (var item in pw)
            {
                ProductWashingVIewModel pwvm = new ProductWashingVIewModel
                {
                    ProductWashingID = item.ProductWashingID,
                    ProductID = item.ProductID,
                    WashingID = item.WashingID,
                    CreateDate = item.CreateDate,
                    WashingName = ws.Where(x => x.WashingID == item.WashingID).FirstOrDefault().WashingName,
                };

                pwvmlist.Add(pwvm);
            }

            return Json(pwvmlist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(int pid, int wid)
        {
            try
            {
                if (db.ProductWashings.Any(x => x.ProductID == pid && x.WashingID == wid))
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "洗滌條件重覆選取 !",
                    });
                }

                ProductWashing pw = new ProductWashing
                {
                    ProductID = pid,
                    WashingID = wid
                };

                db.ProductWashings.Add(pw);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = false,
                    Message = ex.ToString(),
                });
            }

            return Json(new
            {
                Status = true,
            });
        }

        //刪除
        public ActionResult Delete(int pwid)
        {
            try
            {
                db.ProductWashings.Remove(db.ProductWashings.Find(pwid));
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = false,
                    Message = ex.ToString(),
                });
            }

            return Json(new
            {
                Status = true,
            });
        }
    }
}