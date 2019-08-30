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
    public class SizeController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/Size
        public ActionResult Index(int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            return View(db.Sizes.ToList().ToList().ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Size s)
        {
            Size sz = new Size()
            {
                SizeName = s.SizeName
            };

            db.Sizes.Add(sz);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        [HttpGet]
        public ActionResult Edit(int id = 1)
        {
            return View(db.Sizes.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Size sz)
        {
            db.Entry(sz).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        public ActionResult Delete(int id = 1)
        {
            Size sz = db.Sizes.Find(id);
            db.Sizes.Remove(sz);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }
    }
}