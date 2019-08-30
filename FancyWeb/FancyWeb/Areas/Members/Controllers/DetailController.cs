using FancyWeb.Areas.Members.Service;
using FancyWeb.Areas.Members.ViewModels;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.Members.Controllers
{
    public class DetailController : Controller
    {
        // GET: Member/Detail
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Account()
        {
            if (Request.Cookies["IsLogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        //會員資料
        public ActionResult UserDetail()
        {
            MemberService service = new MemberService();

            int id = Int32.Parse(Request.Cookies["upid"].Value);
            ViewBag.Citylist = new SelectList(service.CityJson(), "id", "name", service.UserDetail(id).City);
            return PartialView(service.UserDetail(id));
        }
        //會員資料
        [HttpPost]
        public ActionResult UserName()
        {
            MemberService service = new MemberService();
            if (Request.Cookies["upid"]==null)
            {
                return Json("");
            }
            int id = Int32.Parse(Request.Cookies["upid"].Value);
            return Json(service.UserDetail(id).UserName);
        }
        //修改會員資料
        [HttpPost]
        public ActionResult UserDetail(MemberDetailView data)
        {
            MemberService service = new MemberService();

            int id = Int32.Parse(Request.Cookies["upid"].Value);
            if (service.ChangeUserDetail(data, id))
            {
                return Json("success");
            }
            else { return Json("fail"); }
        }

        //修改會員頭像
        [HttpPost]
        public ActionResult UPloadUserIMG(HttpPostedFileBase U_Image)
        {
            int id = Int32.Parse(Request.Cookies["upid"].Value);
            if (U_Image != null)
            {
                MemberService service = new MemberService();
                if (service.UploadUIMG(U_Image, id))
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
                return Json("失敗");
            }
        }
        //訂單
        public ActionResult OrderList()
        {
            MemberService service = new MemberService();
            OrderHeaderViewModel model = new OrderHeaderViewModel();
            model.orderList = service.GetOrderHeader(Int32.Parse(Request.Cookies["upid"].Value));
            return PartialView(model);
        }
        //訂單過濾
        public ActionResult OrderListFilter(OrderFilter filter)
        {
            MemberService service = new MemberService();
            filter.list = service.GetOrderHeader(Int32.Parse(Request.Cookies["upid"].Value));
            List<OrderHeaderView> newlist = service.OrderHeaderFilter(filter);
            return Json(newlist, JsonRequestBehavior.AllowGet);
        }
        //訂單詳細
        [HttpPost]
        public ActionResult OrderDetail(int oid)
        {
            MemberService service = new MemberService();
            return Json(service.OrderDetail(oid),JsonRequestBehavior.AllowGet);
        }

        //我的收藏
        public ActionResult Favorite()
        {
            MemberService service = new MemberService();
            FavoriteViewModel model = new FavoriteViewModel();
            model.Favorlist = service.GetFavoritelist(Int32.Parse(Request.Cookies["upid"].Value));
            return PartialView(model);
        }
        //我的收藏 過濾
        public ActionResult FavoriteFilter(FilterClass filter)
        {
            MemberService service = new MemberService();
            filter.list = service.GetFavoritelist(Int32.Parse(Request.Cookies["upid"].Value));
            List<FavoriteView> newlist = service.FavoritesFilter(filter);
            return Json(newlist, JsonRequestBehavior.AllowGet);
        }
        //移除我的收藏
        [HttpPost]
        public ActionResult RemoveFV(int fvid)
        {
            MemberService service = new MemberService();
            if (service.FavoriteRemove(fvid))
            {
                return Json("Done");
            }
            else
            {
                return Json("fail");
            }
        }
        //修改密碼
        public ActionResult ChangPassword()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult ChangPassword(string Email, string OldPW, string NewPW)
        {

            MemberService service = new MemberService();
            MemberService.upid = Request.Cookies["upid"].Value;
            if (service.ChangePW(Email, OldPW, NewPW))
            {
                return Json("success");
            }
            else
            {
                return Json("Fail");
            }
        }
        //圖片
        public ActionResult GetImageByte(int? id = 1)
        {
            MemberService service = new MemberService();
            byte[] img = service.GetUserImg(id);
            return File(img, "image/jpeg");
        }

    }
}