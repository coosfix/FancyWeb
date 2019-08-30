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
    public class ColorController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/Color
        public ActionResult Index(int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            return View(db.Colors.ToList().ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Color c)
        {
            Color clr = new Color()
            {
                ColorName = c.ColorName,
                R = c.R,
                G = c.G,
                B = c.B
            };

            db.Colors.Add(clr);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        [HttpGet]
        public ActionResult Edit(int id = 1)
        {
            return View(db.Colors.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Color clr)
        {
            db.Entry(clr).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        public ActionResult Delete(int id = 1)
        {
            Color clr = db.Colors.Find(id);
            db.Colors.Remove(clr);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }
    }
}