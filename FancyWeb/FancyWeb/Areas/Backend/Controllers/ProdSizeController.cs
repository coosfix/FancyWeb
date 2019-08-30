using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using FancyWeb.Areas.Backend.ViewModels;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class ProdSizeController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/ProdSize
        public ActionResult Index(int id)
        {
            ProductSizeVIewModel psvm = new ProductSizeVIewModel();

            var pd = db.Products.Find(id);

            psvm.ProductID = pd.ProductID;
            psvm.ProductName = pd.ProductName;
            ViewBag.productsize = db.Sizes;

            return View(psvm);
        }

        public ActionResult ProdSizeJson(int id)
        {
            List<ProductSizeVIewModel> psvmlist = new List<ProductSizeVIewModel>();

            var ps = db.ProductSizes.Where(x => x.ProductID == id).OrderByDescending(x => x.ProductSizeID);
            var s = db.Sizes;

            foreach (var item in ps)
            {
                ProductSizeVIewModel psvm = new ProductSizeVIewModel
                {
                    ProductSizeID = item.ProductSizeID,
                    ProductID = item.ProductID,
                    SizeID = item.SizeID,
                    CreateDate = item.CreateDate,
                    SizeName = s.Where(x =>x.SizeID  == item.SizeID).FirstOrDefault().SizeName,
                };

                psvmlist.Add(psvm);
            }

            return Json(psvmlist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(int pid, int sid)
        {
            try
            {
                if (db.ProductSizes.Any(x => x.ProductID == pid && x.SizeID == sid))
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "尺吋大小重覆選取 !",
                    });
                }

                ProductSize ps = new ProductSize
                {
                    ProductID = pid,
                    SizeID = sid
                };

                db.ProductSizes.Add(ps);
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
        public ActionResult Delete(int psid)
        {
            try
            {
                db.ProductSizes.Remove(db.ProductSizes.Find(psid));
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