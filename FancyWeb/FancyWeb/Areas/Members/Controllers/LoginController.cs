using FancyWeb.Areas.Members.Service;
using FancyWeb.Areas.Members.ViewModels;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.Members.Controllers
{
    public class LoginController : Controller
    {

        MemberService service = new MemberService();
        private FancyStoreEntities db = new FancyStoreEntities();
        public ActionResult Index()
        {
            //if (Request.Cookies["IsLogin"] != null)
            //{
            //    return RedirectToAction("Browse", "Product", new { area = "ProductDisplay" });
            //}
            return View();
        }
        [HttpPost]
        public ActionResult Index(string UserName, string Password, bool AutoLogin)
        {
            if (UserName == "" || Password == "")
            {
                return Json("none");
            }
            string status = service.LoginCheck(UserName, Password);
            if (status == "islogin")
            {
                if (AutoLogin) addcookie(7); else addcookie(1);
                if (Session["RequestURL"] != null)
                {
                    string[] CA = Session["RequestURL"].ToString().Split('-');
                    UriBuilder url = new UriBuilder(Request.Url)
                    {
                        Path = Url.Action(CA[1], CA[0], new { area = CA[2] })
                    };
                    return Json(url.ToString());
                }
                else
                {
                    UriBuilder url = new UriBuilder(Request.Url)
                    {
                        Path = Url.Action("Index", "Home", new { area = "HomePage" })
                    };
                    return Json(url.ToString());
                };
            }
            else if (status == "noenabled")
            {
                ViewBag.Message = "你的帳戶已被黑名單";
                return Json("noenabled");
            }
            else
            {
                ViewBag.Message = "帳號密碼錯誤";
                return Json("nologin");
            }
        }
        [HttpPost]
        public ActionResult Register(MemberRegisterView RegisterMember)
        {

            if (service.AccountCheck(RegisterMember.UserName) || service.EmailCheck(RegisterMember.Email))
            {
                return Json("資料重複");
            }
            if (MemberMethod.IsValidEmail(RegisterMember.Email) && MemberMethod.IsValidPhone(RegisterMember.Phone))
            {
                string guid = Guid.NewGuid().ToString("N");
                Photo photo = new Photo
                {
                    Photo1 = db.Photos.Find(1).Photo1,
                    CreateDate = DateTime.Now
                };
                db.Photos.Add(photo);
                db.SaveChanges();
                RegisterMember.newMember = new User()
                {
                    UserName = RegisterMember.UserName,
                    UserPassword = MemberMethod.HashPw(RegisterMember.UserPassword, guid),
                    Email = RegisterMember.Email,
                    GUID = guid,
                    Phone = RegisterMember.Phone,
                    RegistrationDate = DateTime.Now,
                    Enabled = true,
                    RegionID = RegisterMember.Region,
                    OauthType = "N",
                    PhotoID = photo.PhotoID,
                    Address = RegisterMember.Address,
                    Gender = RegisterMember.Gender.Equals("male"),
                    VerificationCode = String.Empty,
                    Destination = "."
                };
                if (service.Register(RegisterMember.newMember))
                {
                    return Json("成功");
                }
                else
                {
                    return Json("失敗");
                }
            }
            else
            {
                return Json("資料格式不正確");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (Request.Cookies["IsLogin"] != null)
            {
                foreach (string key in Request.Cookies.AllKeys)
                {
                    HttpCookie c = Request.Cookies[key];
                    c.Expires = DateTime.Now.AddMonths(-1);
                    Response.AppendCookie(c);
                }
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public ActionResult ForgotPw_step1(string Email)
        {
            if (service.EmailCheck(Email))
            {
                return Json("done");
            }
            else
            {
                return Json("fail");
            }
        }
        [HttpPost]
        public ActionResult ForgotPw_step2(string Account, string Email)
        {
            if (service.AccountCheck(Account))
            {
                if (Email != "" && Account != "")
                {
                    string np = MemberMethod.GetNewPW();
                    string newv = service.UpdatePassword(np, Account);
                    if (newv != null)
                    {
                        string tempmail = System.IO.File.ReadAllText(Server.MapPath(@"~/Areas/Members/Email/verification.html"));//讀取html
                        UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                        {
                            Path = Url.Action("AccountOpen", "Login", new
                            {
                                AuthCode = newv
                            })
                        };
                        MemberMethod.SendEmail("Fancy忘記密碼㊙️", Email, Account, np, MemberMethod.VerificationCodeMailBody(tempmail, Account,
                            np, ValidateUrl.ToString().Replace("%3F", "?")));
                        return Json("done");
                    }
                }
                return Json("fail");
            }
            else
            {
                return Json("fail");
            }
        }

        public ActionResult AccountOpen(string AuthCode = "")
        {
            if (service.ClearV(AuthCode))
            {
                ViewBag.Message = "Succes";
            }
            else
            {
                ViewBag.Message = "fail";
            }
            return View();
        }
        public ActionResult OAuthLogin(string Method)
        {
            switch (Method)
            {
                case "LINE":
                    return Redirect(OAuthRequestUrl.LineUrl());
                case "Facebook":
                    return Redirect(OAuthRequestUrl.FBeUrl());
                case "Google":
                    return Redirect(OAuthRequestUrl.GoogleeUrl());
                default:
                    return View("Index");
            }
        }
        public ActionResult callback(string code, string state)
        {
            if (code != null)
            {
                Dictionary<string, string> UserData = new Dictionary<string, string>();
                switch (state.Split('-')[1])
                {
                    case "LINE":
                        UserData = OAuthMethod.LineResponse(code, state);
                        break;
                    case "Facebook":
                        UserData = OAuthMethod.FBResponse(code, state);
                        break;
                    case "Google":
                        UserData = OAuthMethod.GoogleResponse(code, state);
                        break;
                    default:
                        break;
                }
                if (service.LoginCheck(UserData["name"], UserData["ID"]) == "islogin")
                {
                    addcookie(7);
                    HttpCookie userimg = new HttpCookie("userimg")
                    {
                        Value = UserData["picture"],
                        Expires = DateTime.Now.AddDays(7)
                    };
                    Response.Cookies.Add(userimg);
                    return RedirectToAction("Index", "Home", new { area = "HomePage" });
                }
                else
                {
                    string guid = Guid.NewGuid().ToString("N");
                    Photo photo = new Photo
                    {
                        Photo1 = Encoding.UTF8.GetBytes(UserData["picture"]),
                        CreateDate = DateTime.Now
                    };
                    db.Photos.Add(photo);
                    db.SaveChanges();
                    Models.User user = new User()
                    {
                        UserName = UserData["name"],
                        UserPassword = MemberMethod.HashPw(UserData["ID"], guid),
                        Email = UserData["email"],
                        GUID = guid,
                        Phone = "0912345678",
                        PhotoID = photo.PhotoID,
                        RegistrationDate = DateTime.Now,
                        Enabled = true,
                        RegionID = 1,
                        Address = "",
                        OauthType = state.Split('-')[1].Substring(0, 1),
                        VerificationCode = String.Empty,
                        Gender = true
                    };

                    if (service.Register(user))
                    {
                        addcookie(7);
                        HttpCookie userimg = new HttpCookie("userimg")
                        {
                            Value = UserData["picture"],
                            Expires = DateTime.Now.AddDays(7)
                        };
                        Response.Cookies.Add(userimg);
                        return RedirectToAction("Index", "Home", new { area = "HomePage" });
                    }
                }
                return RedirectToAction("Index", "Login", new { area = "Members" });
            }
            else
            {
                return View("Index");
            }
        }

        public void addcookie(int day)
        {
            HttpCookie LoginCookie = new HttpCookie("IsLogin")
            {
                Value = "GoodBoy",
                Expires = DateTime.Now.AddDays(day)
            };
            HttpCookie upid = new HttpCookie("upid")
            {
                Value = MemberService.upid,
                Expires = DateTime.Now.AddDays(day)
            };
            if (service.isAdmin(MemberService.upid))
            {
                HttpCookie isadmin = new HttpCookie("isadmin")
                {
                    Value = MemberService.upid,
                    Expires = DateTime.Now.AddDays(day)
                };
                Response.Cookies.Add(isadmin);
            }
            if (service.isService(MemberService.upid))
            {
                HttpCookie isService = new HttpCookie("isService")
                {
                    Value = MemberService.upid,
                    Expires = DateTime.Now.AddDays(day)
                };
                Response.Cookies.Add(isService);
            }
            Response.Cookies.Add(LoginCookie);
            Response.Cookies.Add(upid);
        }
        public ActionResult ReturnCity()
        {
            MemberService service = new MemberService();
            return Json(service.CityJson(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReturnRegion(int? cid)
        {
            MemberService service = new MemberService();
            return Json(service.RegionJson(cid), JsonRequestBehavior.AllowGet);
        }
    }
}