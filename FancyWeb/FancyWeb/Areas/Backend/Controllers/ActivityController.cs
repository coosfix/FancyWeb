using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using FancyWeb.Areas.Backend.ViewModels;
using PagedList;
using PagedList.Mvc;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class ActivityController : Controller
    {
        private FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/Activity
        public ActionResult Index(int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            var act = db.Activities.ToList();
            List<ActivityViewModel> activityList = new List<ActivityViewModel>();
            var dis = db.DiscountMethods;

            foreach (var item in act)
            {
                ActivityViewModel av = new ActivityViewModel();

                av.ActivityID = item.ActivityID;
                av.ActivityName = item.ActivityName;
                av.DiscountID = item.DiscountID;
                av.BeginDate = item.BeginDate;
                av.EndDate = item.EndDate;
                av.CreateDate = item.CreateDate;
                av.DiscountName = dis.Where(x => x.DiscountID == item.DiscountID).FirstOrDefault().DiscountName;

                activityList.Add(av);
            }

            return View(activityList.OrderByDescending(x=>x.ActivityID).ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        public ActionResult Create()
        {

            ViewBag.datas = db.DiscountMethods;
            return View();
        }

        [HttpPost]
        public ActionResult Create(ActivityViewModel avm)
        {
            Activity av = new Activity()
            {
                ActivityID = avm.ActivityID,
                ActivityName = avm.ActivityName,
                BeginDate = avm.BeginDate,
                EndDate = avm.EndDate,
                DiscountID = avm.DiscountID,
                CreateDate = DateTime.Now
            };

            db.Activities.Add(av);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id, int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            var av = db.Activities.Find(id ?? 1);

            ActivityEditViewModel aevm = new ActivityEditViewModel();
            List<ActivityProdViewModel> apList = new List<ActivityProdViewModel>();

            aevm.ActivityID = av.ActivityID;
            aevm.ActivityName = av.ActivityName;
            aevm.BeginDate = av.BeginDate;
            aevm.EndDate = av.EndDate;
            aevm.DiscountID = av.DiscountID;
            aevm.DiscountName = db.DiscountMethods.Find(av.DiscountID).DiscountName;
            aevm.CreateDate = av.CreateDate;

            var ap = db.ActivityProducts.Where(x => x.ActivityID == id);
            List<Product> prod = db.Products.ToList();
            List<VW_EW_CategorySM> cateMS = db.VW_EW_CategorySM.ToList();
            List<Supplier> suppliers = db.Suppliers.ToList();
            List<ProductPhoto> prodPhoto = db.ProductPhotoes.ToList();


            foreach (var item in ap)
            {
                ActivityProdViewModel apvm = new ActivityProdViewModel();

                apvm.ActivityID = item.ActivityID;
                apvm.ProductID = item.ProductID;
                var pd = prod.Where(x => x.ProductID == item.ProductID).FirstOrDefault();
                apvm.ProductName = pd.ProductName;
                apvm.CategoryMSName = cateMS.Where(x => x.CategorySID == pd.CategorySID).FirstOrDefault().CategoryName;
                apvm.SupplierName = suppliers.Where(x => x.SupplierID == pd.SupplierID).FirstOrDefault().SupplierName;

                if (prodPhoto.Any(x => x.ProductID == item.ProductID && ((x.PhotoID == null ? 0 : x.PhotoID) > 0)))
                {
                    apvm.PhotoID = (int)prodPhoto.Where(x => x.ProductID == item.ProductID).FirstOrDefault().PhotoID;
                }
                //aevm.producstList
                apList.Add(apvm);
            }

            aevm.producstList = apList;

            ViewBag.datas = db.DiscountMethods;
            return View(aevm);
        }

        //修改完後,還會停在原來頁面
        [HttpPost]
        public ActionResult Edit(ActivityViewModel avm, int? page)
        {
            var av = db.Activities.Find(avm.ActivityID);
            av.ActivityName = avm.ActivityName;
            av.BeginDate = avm.BeginDate;
            av.EndDate = avm.EndDate;
            av.ActivityID = avm.ActivityID;
            av.DiscountID = avm.DiscountID;

            db.Entry(av).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            TempData["savepage"] = page ?? 1;
            return RedirectToAction("Edit", new { id = avm.ActivityID, page = TempData["savepage"] });
        }

        //刪除活動(會連下面的Product全部一起刪除)
        [HttpPost]
        public ActionResult Delete(int? id, int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;
            id = id ?? 0;
            if (id == 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "id 為 0, 無法刪除 !",
                });
            }
            try
            {
                if (db.ActivityProducts.Any(x => x.ActivityID == id))
                {
                    List<ActivityProduct> ap = db.ActivityProducts.Where(x => x.ActivityID == id).ToList();

                    foreach (var item in ap)
                    {
                        var apt = db.ActivityProducts.Find(item.ActivityDetailID);
                        db.ActivityProducts.Remove(apt);
                    }
                }

                var atv = db.Activities.Find(id);
                db.Activities.Remove(atv);
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

        //刪除活動下的Product
        public ActionResult DeleteProd(int actid, int prodid, int page)
        {
            var av = db.ActivityProducts.Where(x => x.ActivityID == actid && x.ProductID == prodid).FirstOrDefault();

            db.ActivityProducts.Remove(av);
            db.SaveChanges();

            TempData["savepage"] = page;
            return RedirectToAction("Edit", new { id = actid, page = TempData["savepage"] });
        }
    }
}