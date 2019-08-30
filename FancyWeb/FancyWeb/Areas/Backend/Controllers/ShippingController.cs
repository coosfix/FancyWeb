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
    public class ShippingController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/Shipping
        public ActionResult Index(int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            return View(db.Shippings.ToList().ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Shipping s)
        {
            Shipping sp = new Shipping()
            {
                ShippingName = s.ShippingName,
                Phone = s.Phone,
                Fax = s.Fax,
                Email = s.Email,
                Address = s.Address,
                CreateDate = DateTime.Now
            };

            db.Shippings.Add(sp);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        [HttpGet]
        public ActionResult Edit(int id = 1)
        {
            return View(db.Shippings.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Shipping sp)
        {
            db.Entry(sp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        public ActionResult Delete(int id = 1)
        {
            Shipping sp = db.Shippings.Find(id);
            db.Shippings.Remove(sp);
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }
    }
}