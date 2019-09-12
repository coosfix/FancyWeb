using FancyWeb.Areas.Management.Service;
using FancyWeb.Areas.Management.ViewModels;
using FancyWeb.Areas.Members.Service;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.Management.Controllers
{
    public class MainController : Controller
    {
        // GET: Management/Main
        private FancyStoreEntities db = new FancyStoreEntities();

        public ActionResult Index()
        {
            AdminService admin = new AdminService();
            MemberService service = new MemberService();
            ViewBag.Citylist = new SelectList(service.CityJson(), "id", "name", 1);
            return View(admin.GetUserList());
        }

        //管理員針對使用者修改密碼
        [HttpPost]
        public ActionResult AdminControllsPW(int id)
        {
            AdminService admin = new AdminService();
            string tempmail = System.IO.File.ReadAllText(Server.MapPath(@"~/Areas/Management/Email/verification2.html"));//讀取html
            string NewPW = MemberMethod.GetNewPW();
            string guid = Guid.NewGuid().ToString("N");
            UriBuilder ValidateUrl = new UriBuilder(Request.Url)
            {
                Path = Url.Action("Index", "Login",new { area="Members"})
            };
            if (admin.AdminUserControl(id, NewPW, guid, tempmail, ValidateUrl.ToString()))
            {
                return Json("Success");
            }
            else { return Json("Fail"); }
        }
      
        //黑名單會員
        [HttpPost]
        public ActionResult BanControl(int id)
        {
            AdminService admin = new AdminService();
            if (admin.DoBan(id))
            {
                return Json("success");
            }
            else
            {
                return Json("fail");
            }
        }
        //寄送訊息給使用者
        [HttpPost]
        public ActionResult SendEmail(string UserName, string Email, string content)
        {
            AdminService admin = new AdminService();
            string tempmail = System.IO.File.ReadAllText(Server.MapPath(@"~/Areas/Management/Email/template_Message.html"));//讀取html

            tempmail = tempmail.Replace("{{使用者名稱}}", UserName);
            tempmail = tempmail.Replace("{{內容}}", content);
            UriBuilder uriBuilder = new UriBuilder(Request.Url) {
                Path = Url.Action("Index","Login",new { area = "Members"})
            };
            tempmail = tempmail.Replace("{{網址}}", uriBuilder.ToString());
            if (admin.SendEmail(tempmail, Email, UserName, content))
            {
                return Json("Success");
            }
            return Json("Fail");
        }
        //新增員工
        [HttpPost]
        public ActionResult RegisterAdmin(RegisterAdminModel data)
        {
            string guid = Guid.NewGuid().ToString("N");
            AdminService admin = new AdminService();
            Photo photo = new Photo
            {
                Photo1 = db.Photos.Find(1).Photo1,
                CreateDate = DateTime.Now
            };
            db.Photos.Add(photo);
            db.SaveChanges();
            User user = new User()
            {
                UserName = data.UserName,
                UserPassword = MemberMethod.HashPw(data.UserPassword, guid),
                Email = data.Email,
                GUID = guid,
                Phone = data.Phone,
                RegistrationDate = DateTime.Now,
                Enabled = true,
                PhotoID = photo.PhotoID,
                Admin = true,
                RegionID = data.Region,
                VerificationCode = "",
                Address = "",
                OauthType = "N",
                Gender = data.Gender.Equals("male"),
                Destination="."
            };
            string uid = admin.Register(user).ToString();
            return uid != "0" ? Json(uid) : Json("Fail");
        }
        [HttpPost]
        public ActionResult UserDetail(int id)
        {
            AdminService service = new AdminService();
            return Json(service.GetUserDetial(id), JsonRequestBehavior.AllowGet);
        }
        //頭像
        public ActionResult GetImageByte(int id = 1)
        {
            AdminService service = new AdminService();
            byte[] img = service.GetUserImg(id);
            return File(img, "image/jpeg");
        }
    }
}