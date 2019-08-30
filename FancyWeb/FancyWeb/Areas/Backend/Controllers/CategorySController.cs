using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using PagedList;
using PagedList.Mvc;
using FancyWeb.Areas.Backend.ViewModels;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class CategorySController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/CategoryS
        public ActionResult Index(int? page, int? catemid)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            //記錄目前中分類ID,若是空值就給第一筆的ID
            var cid = catemid ?? db.CategoryMiddles.First().CategoryMID;
            TempData["savecatemid"] = cid;

            var cs = db.CategorySmalls.Where(x => x.CategoryMID == cid);
            var cm = db.CategoryMiddles.ToList();
            List<CategorySViewModel> categorySList = new List<CategorySViewModel>();

            foreach (var item in cs)
            {
                CategorySViewModel csm = new CategorySViewModel();
                csm.CategorySID = item.CategorySID;
                csm.CategorySName = item.CategorySName;
                csm.CategoryMID = item.CategoryMID;
                csm.CategoryMName = cm.Where(x => x.CategoryMID == item.CategoryMID).FirstOrDefault().CategoryMName;
                categorySList.Add(csm);
            }

                return View(categorySList.ToPagedList(page ?? 1, 5));
        }

        public ActionResult CategoryMJson()
        {
            var cate = db.CategoryMiddles.Select(x => new { x.CategoryMID, x.CategoryMName });

            return Json(cate, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create(int? catemid)
        {
            //記錄目前中分類ID,若是空值就給第一筆的ID
            var cid = catemid ?? db.CategoryMiddles.First().CategoryMID;
            TempData["savecatemid"] = cid;

            CategorySViewModel csm = new CategorySViewModel();

            csm.CategoryMID = cid;
            csm.CategoryMName = db.CategoryMiddles.Find(cid).CategoryMName;

            return View(csm);
        }

        [HttpPost]
        public ActionResult Create(CategorySViewModel categoryS)
        {
            CategorySmall cate = new CategorySmall()
            {
                CategorySName = categoryS.CategorySName,
                CategoryMID = categoryS.CategoryMID
            };

            db.CategorySmalls.Add(cate);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"], catemid = categoryS.CategoryMID });
        }

        [HttpGet]
        public ActionResult Edit(int? id, int? catemid)
        {
            //記錄目前中分類ID,若是空值就給第一筆的ID
            var cid = catemid ?? db.CategoryMiddles.First().CategoryMID;
            TempData["savecatemid"] = cid;

            var cs = db.CategorySmalls.Find(id);

            CategorySViewModel csm = new CategorySViewModel();

            csm.CategorySID = cs.CategorySID;
            csm.CategorySName = cs.CategorySName;
            csm.CategoryMID = cid;
            csm.CategoryMName = db.CategoryMiddles.Find(cid).CategoryMName;

            return View(csm);
        }

        [HttpPost]
        public ActionResult Edit(CategorySViewModel csm)
        {
            CategorySmall cs = db.CategorySmalls.Find(csm.CategorySID);
            cs.CategorySName = csm.CategorySName;

            db.Entry(cs).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"], catemid = csm.CategoryMID });
        }

        public ActionResult Delete(int id = 1)
        {
            var cs = db.CategorySmalls.Find(id);

            db.CategorySmalls.Remove(cs);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"], catemid = cs.CategoryMID });
        }
    }
}