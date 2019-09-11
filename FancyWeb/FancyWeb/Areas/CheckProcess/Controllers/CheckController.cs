using FancyWeb.Areas.CheckProcess.Function;
using FancyWeb.Areas.ProductDisplay.Models;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.CheckProcess.Controllers
{
    public class CheckController : Controller
    {
        public FancyStoreEntities db = new FancyStoreEntities();
        // GET: CheckProcess/Check

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail()
        {
            if (Session["recent"] != null)
            {
                List<int> recent = (List<int>)Session["recent"];

                List<CartItem> citems = new List<CartItem>();
                CartItem citem;

                foreach (var pd in recent)
                {
                    decimal sprice;
                    string activityname;
                    if (db.ActivityProducts.Where(a => a.ProductID == pd).Count() > 0)
                    {
                        sprice = db.ActivityProducts.Where(a => a.ProductID == pd && a.Activity.EndDate >= DateTime.Now).FirstOrDefault().Activity.DiscountMethod.Discount;
                        activityname = db.ActivityProducts.Where(a => a.ProductID == pd && a.Activity.EndDate >= DateTime.Now).FirstOrDefault().Activity.ActivityName;
                    }
                    else
                    {
                        sprice = 0;
                        activityname = null;
                    }
                    citem = new CartItem()
                    {
                        ProductID = pd,
                        ProductName = db.Products.Find(pd).ProductName,
                        UnitPrice = db.Products.Find(pd).UnitPrice,
                        SUnitPrice = Convert.ToInt32(Math.Floor(sprice * db.Products.Find(pd).UnitPrice)),
                        ActivityName = activityname
                    };
                    citems.Add(citem);
                }

                ViewBag.recent = citems.ToList();
            }

            ViewBag.paymethod = db.PayMethods.ToList();
            if (Session["IsLogin"] != null || Request.Cookies["IsLogin"] != null)
            {
                List<CartItem> cart;
                int uid = int.Parse(Request.Cookies["upid"].Value);
                var usercart = db.Carts.Where(c => c.UserID == uid).ToList();
                if (usercart.Count() > 0)
                {
                    cart = ProductMethod.Cart(uid);
                    Session.Add("check", cart);
                    return View(cart);
                }
                else
                {
                    cart = new List<CartItem>();
                    return View(cart);
                }
            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "Members" });
            }
        }

        public ActionResult ChangeQty(int id, int cid, int sid, int qty)
        {
            List<CartItem> cart;
            int uid = int.Parse(Request.Cookies["upid"].Value);
            var sameitem = db.Carts.Where(c => c.UserID == uid && c.ProductID == id && c.ProductColorID == cid && c.ProductSizeID == sid).First();
            sameitem.Quantity = qty;
            db.Entry(sameitem).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            cart = ProductMethod.Cart(uid);
            return Json(cart, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckStock(int id, int cid, int sid, int qty)
        {
            var stock = db.ProductStocks.Where(s => s.ProductID == id && s.ProductSizeID == sid && s.ProductColorID == cid).FirstOrDefault().StockQTY;

            bool enough = (stock < qty) ? false : true;

            return Json(enough, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveCart(int id, int cid, int sid)
        {
            List<CartItem> cart;
            int uid = int.Parse(Request.Cookies["upid"].Value);
            var cartitem = db.Carts.Where(c => c.UserID == uid && c.ProductID == id && c.ProductColorID == cid && c.ProductSizeID == sid).First();
            db.Carts.Remove(cartitem);
            db.SaveChanges();
            var usercart = db.Carts.Where(c => c.UserID == uid).ToList();
            if (usercart.Count() > 0)
            {
                //把db.cart內容回傳cart>
                cart = ProductMethod.Cart(uid);
                return Json(cart, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CartDetail(OrderHeader orderHeader, List<CartItem> cartItems)
        {
            Session.Add("orderHeader", orderHeader);
            Session.Add("cartItems", cartItems);
            return Json("sucess");
        }

        public ActionResult OrderInfo()
        {
            if (Session["orderHeader"] != null && Session["cartItems"] != null)
            {
                ViewBag.orderHeader = (OrderHeader)Session["orderHeader"];
                int payid = ((OrderHeader)Session["orderHeader"]).PayMethodID;
                int carttotal = ((OrderHeader)Session["orderHeader"]).OrderAmount;
                int? pay = db.PayMethods.Find(payid).Freight;
                ViewBag.pay = pay;
                ViewBag.carttotal = carttotal - pay;
                ViewBag.cartItems = (List<CartItem>)Session["cartItems"];
                ViewBag.city = db.Cities.ToList();
                //ViewBag.region = db.Regions.ToList();
                int uid = int.Parse(Request.Cookies["upid"].Value);
                var user = db.Users.Where(u => u.UserID == uid).First();
                return View(user);
            }
            else
            {
                return RedirectToAction("Detail", "Check");
            }
        }

        public ActionResult GetRegionByID(int id)
        {
            var regions = db.Regions.Where(r => r.CityID == id).Select(r => new { r.RegionID, r.RegionName });
            return Json(regions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendOrder(OrderHeader orderHeader, List<CartItem> cartItems)
        {
            int uid = int.Parse(Request.Cookies["upid"].Value);
            orderHeader.OrderNum = $"GD{DateTime.Now:yyyyMMddHHmmss}{uid}";
            orderHeader.OrderDate = DateTime.Now;
            orderHeader.UserID = uid;
            orderHeader.ShippingID = 1;
            orderHeader.OrderStatusID = 1;
            orderHeader.CreateDate = DateTime.Now;

            db.OrderHeaders.Add(orderHeader);
            db.SaveChanges();

            var orderid = db.OrderHeaders.OrderByDescending(o => o.OrderID).FirstOrDefault().OrderID;
            var ordernum = db.OrderHeaders.OrderByDescending(o => o.OrderID).FirstOrDefault().OrderNum;

            string tempmail = System.IO.File.ReadAllText(Server.MapPath(@"~/Areas/CheckProcess/Email/ordersucess.html"));//讀取html

            UriBuilder ValidateUrl = new UriBuilder(Request.Url)
            {
                Path = Url.Action("Account", "Detail", new
                {
                    area = "Members",
                    ad = "ord"
                })
            };
            string email = db.Users.Find(uid).Email;

            CheckMethod.SendEmail(email, CheckMethod.VerificationCodeMailBody(tempmail, orderHeader, ValidateUrl.ToString().Replace("%3F", "?")));

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            OrderDetail orderDetail;
            foreach (var cartItem in cartItems)
            {
                orderDetail = new OrderDetail()
                {
                    OrderID = orderid,
                    ProductID = cartItem.ProductID,
                    ProductColorID = cartItem.ProductColorID,
                    ProductSizeID = cartItem.ProductSizeID,
                    UnitPrice = cartItem.UnitPrice,
                    OrderQTY = cartItem.OrderQTY,
                    CreateDate = DateTime.Now,
                    DiscountID = cartItem.DiscountID
                };
                orderDetails.Add(orderDetail);
                var stockqty = db.ProductStocks.Where(s => s.ProductID == orderDetail.ProductID && s.ProductColorID == orderDetail.ProductColorID && s.ProductSizeID == orderDetail.ProductSizeID).FirstOrDefault().StockQTY;
                if (orderDetail.OrderQTY > stockqty)
                {
                    string productname = db.Products.Find(orderDetail.ProductID).ProductName;
                    UserNotice notice = new UserNotice()
                    {
                        UserID = uid,
                        Comment = $"訂單 [ {ordernum } ] 中 [ {productname} ] 已經為您完成預購，預購商品預計14~30天完成追加到貨，後續將會依序安排出貨，還請留意出貨通知",
                        IsRead = false,
                        NoticeDate = DateTime.Now
                    };
                    db.UserNotices.Add(notice);
                    db.SaveChanges();
                }
            }
            db.OrderDetails.AddRange(orderDetails);
            db.SaveChanges();
            var carts = db.Carts.Where(c => c.UserID == uid).ToList();
            foreach (var cart in carts)
            {
                db.Carts.Remove(cart);
            }
            db.SaveChanges();
            Session.Remove("orderHeader");
            Session.Remove("cartItems");
            TempData.Add("ordernum", ordernum);
            return Json(ordernum, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FinalShow(string ordernum)
        {
            var final = db.OrderHeaders.Where(o => o.OrderNum == ordernum).FirstOrDefault();
            var details = db.OrderDetails.Where(o => o.OrderHeader.OrderNum == ordernum).ToList();
            ViewBag.details = details;
            return View(final);
        }
    }
}