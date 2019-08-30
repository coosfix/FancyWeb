using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class ChatController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/Char
        public ActionResult Index()
        {
            //Login ID
            ViewBag.uid = Int32.Parse(Request.Cookies["upid"].Value);
            User u = db.Users.Find(ViewBag.uid);
            ViewBag.uname = u.UserName;

            //service ID (客服)
            User su = db.Users.Where(x => x.UserName == "service").FirstOrDefault();
            ViewBag.suid = su.UserID;
            ViewBag.suname = su.UserName;

            return PartialView();
        }
        public ActionResult Main()
        {
            return View();
        }
    }
}