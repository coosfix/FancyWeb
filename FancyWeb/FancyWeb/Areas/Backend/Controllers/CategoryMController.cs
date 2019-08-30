using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using PagedList;
using PagedList.Mvc;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class CategoryMController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/CategoryM
        public ActionResult Index(int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            return View(db.CategoryMiddles.ToList().ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CategoryMiddle categoryM)
        {
            CategoryMiddle cate = new CategoryMiddle()
            {
                CategoryMName = categoryM.CategoryMName
            };

            db.CategoryMiddles.Add(cate);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        [HttpGet]
        public ActionResult Edit(int id = 1)
        {
            return View(db.CategoryMiddles.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(CategoryMiddle cate)
        {
            db.Entry(cate).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        public ActionResult Delete(int id = 1)
        {
            CategoryMiddle cate = db.CategoryMiddles.Find(id);
            db.CategoryMiddles.Remove(cate);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }
    }
}