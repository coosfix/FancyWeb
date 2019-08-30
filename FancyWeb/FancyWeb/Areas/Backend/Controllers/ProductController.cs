using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Areas.Backend.ViewModels;
using FancyWeb.Models;
using PagedList;
using PagedList.Mvc;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class ProductController : Controller
    {
        private FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/Product
        public ActionResult Index(int? page, int? catesid)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            //記錄目前中分類ID,若是空值就給第一筆的ID
            if (Session["prodcmid"] == null)
            {
                Session["prodcmid"] = db.CategoryMiddles.FirstOrDefault().CategoryMID;
            }

            var cmid = (int)Session["prodcmid"];

            //記錄目前小分類ID,若是空值就給第一筆的ID
            if (catesid == null)
            {
                if (Session["prodcsid"] != null)
                {
                    catesid = (int)Session["prodcsid"];
                }
                else
                {
                    catesid = db.CategorySmalls.Where(x => x.CategoryMID == cmid).FirstOrDefault().CategorySID;
                }
            }
            Session["prodcsid"] = catesid;

            var prod = db.Products.OrderByDescending(x => x.ProductID).Where(x => x.CategorySID == catesid).ToList();
            var cateMS = db.VW_EW_CategorySM.ToList();
            var supplier = db.Suppliers.ToList();
            var pc = db.ProductColors.ToList();
            List<ProductViewModel> prodList = new List<ProductViewModel>();

            foreach (var item in prod)
            {
                ProductViewModel pd = new ProductViewModel();

                pd.ProductID = item.ProductID;
                pd.ProductName = item.ProductName;
                pd.Desctiption = item.Desctiption;
                pd.CategorySID = item.CategorySID;
                pd.SupplierID = item.SupplierID;
                pd.UnitPrice = item.UnitPrice;
                pd.ProductInDate = item.ProductInDate;
                pd.ProductOutDate = item.ProductOutDate;
                pd.CreateDate = item.CreateDate;

                if (pc.Any(x => x.ProductID == item.ProductID&&((x.PhotoID==null?0:x.PhotoID) > 0)))
                {
                    pd.PhotoID = (int)pc.Where(x => x.ProductID == item.ProductID).FirstOrDefault().PhotoID;
                }

                var c = cateMS.Where(x => x.CategorySID == item.CategorySID).FirstOrDefault();
                pd.CategoryMSName = c.CategoryName;

                var s = supplier.Where(x => x.SupplierID == item.SupplierID).FirstOrDefault();
                pd.SupplierName = s.SupplierName;

                prodList.Add(pd);
            }

            return View(prodList.ToPagedList(page ?? 1, 5));
        }

        public ActionResult CategoryMJson()
        {
            var categoryM = db.CategoryMiddles.Select(c => new
            {
                c.CategoryMID,
                c.CategoryMName
            });

            return Json(categoryM, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CategorySJson(int? id)
        {
            //記錄目前中分類ID,若是空值就給第一筆的ID
            if (id == null)
            {
                id = db.CategoryMiddles.FirstOrDefault().CategoryMID;
            }

            var categoryS = db.CategorySmalls.Where(x => x.CategoryMID == id).Select(c => new
            {
                c.CategorySID,
                c.CategorySName
            });

            Session["prodcmid"] = id;

            return Json(categoryS, JsonRequestBehavior.AllowGet);
        }

        //讀取Activity
        public ActionResult ActivityJson(int prodid)
        {
            var actlist = db.Activities.Select(x => new
            {
                actid = x.ActivityID,
                actname = x.ActivityName,
                chk = db.ActivityProducts.Any(a => a.ActivityID == x.ActivityID && a.ProductID == prodid)
            }).ToList();

            return Json(actlist, JsonRequestBehavior.AllowGet);
        }

        //更改產品歸屬的活動(一個Product能存在一個Activity中)
        [HttpPost]
        public ActionResult UpdateActivity(int prodid = 0, int activityid = 0)
        {
            if (prodid == 0 || activityid == 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "沒有選擇活動, 無法更改!",
                });
            }
            try
            {
                //將不該有的Activity中的Product刪除
                if (db.ActivityProducts.Any(x => x.ProductID == prodid && x.ActivityID != activityid))
                {
                    List<ActivityProduct> aplist = db.ActivityProducts.Where(x => x.ProductID == prodid && x.ActivityID != activityid).ToList();

                    foreach (var item in aplist)
                    {
                        var apt = db.ActivityProducts.Find(item.ActivityDetailID);
                        db.ActivityProducts.Remove(apt);
                    }
                }

                //若不存在,則須在Activity中增加Product
                if (!db.ActivityProducts.Any(x => x.ProductID == prodid && x.ActivityID == activityid))
                {
                    ActivityProduct ap = new ActivityProduct();
                    ap.ActivityID = activityid;
                    ap.ProductID = prodid;
                    ap.CreateDate = DateTime.Now;
                    db.ActivityProducts.Add(ap);
                    db.SaveChanges();
                }
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

        [HttpGet]
        public ActionResult Create()
        {

            ViewBag.categories = db.VW_EW_CategorySM;
            ViewBag.suppliers = db.Suppliers;
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProductViewModel pvm)
        {
            Product pd = new Product()
            {
                ProductName = pvm.ProductName,
                Desctiption = pvm.Desctiption,
                UnitPrice = pvm.UnitPrice,
                CategorySID = pvm.CategorySID,
                SupplierID = pvm.SupplierID,
                ProductInDate = pvm.ProductInDate,
                ProductOutDate = pvm.ProductOutDate,
                CreateDate = DateTime.Now
            };

            db.Products.Add(pd);
            db.SaveChanges();

            Session["prodcsid"] = pvm.CategorySID;

            //return RedirectToAction("Index", new { catesid = pvm.CategorySID });
            return RedirectToAction("Edit", new { id = pd.ProductID, page = TempData["savepage"] });
        }

        [HttpGet]
        public ActionResult Edit(int? id, int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            ViewBag.categories = db.VW_EW_CategorySM;
            ViewBag.suppliers = db.Suppliers;

            var pd = db.Products.Find(id ?? 1);

            ProductViewModel pvm = new ProductViewModel();
            pvm.ProductID = pd.ProductID;
            pvm.ProductName = pd.ProductName;
            pvm.Desctiption = pd.Desctiption;
            pvm.CategorySID = pd.CategorySID;
            pvm.SupplierID = pd.SupplierID;
            pvm.UnitPrice = pd.UnitPrice;
            pvm.ProductInDate = pd.ProductInDate;
            pvm.ProductOutDate = pd.ProductOutDate;

            return View(pvm);
        }

        [HttpPost]
        public ActionResult Edit(ProductViewModel pvm, int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            var pd = db.Products.Find(pvm.ProductID);

            pd.ProductName = pvm.ProductName;
            pd.Desctiption = pvm.Desctiption;
            pd.CategorySID = pvm.CategorySID;
            pd.SupplierID = pvm.SupplierID;
            pd.UnitPrice = pvm.UnitPrice;
            pd.ProductInDate = pvm.ProductInDate;
            pd.ProductOutDate = pvm.ProductOutDate;

            db.Entry(pd).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { catesid = pvm.CategorySID, page= TempData["savepage"] });
        }

    }
}