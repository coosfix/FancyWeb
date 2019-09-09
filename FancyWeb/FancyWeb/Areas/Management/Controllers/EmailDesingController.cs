using FancyWeb.Areas.Management.Service;
using FancyWeb.Areas.Members.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.Management.Controllers
{
    public class EmailDesignController : Controller
    {
        // GET: Management/EmailDesing
        public ActionResult Index()
        {
            EmailDataList service = new EmailDataList();

            return View(service.GetALLProudct());
        }
        //回傳新區塊
        public ActionResult template_newitem()
        {
            return PartialView();
        }
        public ActionResult template_newitem2()
        {
            return PartialView();
        }
        //取得email範本
        [HttpPost]
        public ActionResult GetDoneEmail(string num)
        {
            UriBuilder uriBuilder = new UriBuilder(Request.Url);
            string imgurl = $"{uriBuilder.Scheme}://{uriBuilder.Host}:{uriBuilder.Port}";
            string emailbody = num.Replace("{{imgurl}}", imgurl);
            MemberMethod.SendEmail("Fancy電子報📰", "marybuyfancy@gmail.com", "", "", emailbody);
            return Json("done");
        }

        //取得base64
        public ActionResult Gebase64(int id, int? colorid = 0)
        {
            AdminService service = new AdminService();

            return Json(service.Getbase64(id, colorid),JsonRequestBehavior.AllowGet);
        }
    }
}