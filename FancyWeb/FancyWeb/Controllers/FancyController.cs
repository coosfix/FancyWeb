using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Controllers
{
    public class FancyController : Controller
    {
        // GET: Fancy
        public ActionResult Index()
        {

            return View();
        }
    }
}