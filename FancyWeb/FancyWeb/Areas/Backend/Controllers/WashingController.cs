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
    public class WashingController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/Washing
        public ActionResult Index(int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            return View(db.Washings.ToList().ToList().ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Washing ws)
        {
            Washing washing = new Washing()
            {
                WashingName = ws.WashingName
            };

            db.Washings.Add(washing);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        [HttpGet]
        public ActionResult Edit(int id = 1)
        {
            return View(db.Washings.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Washing ws)
        {
            db.Entry(ws).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        public ActionResult Delete(int id = 1)
        {
            Washing ws = db.Washings.Find(id);
            db.Washings.Remove(ws);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }
    }
}