using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using FancyWeb.Areas.ProductDisplay.Models;
using FancyWeb.Areas.ProductDisplay.ViewModels;
using System.ComponentModel;

namespace FancyWeb.Areas.ProductDisplay.Controllers
{
    public class ProductController : Controller
    {
        public FancyStoreEntities db = new FancyStoreEntities();

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        //商品顯示
        public ActionResult Browse(int id = 1, int? sid = 0)
        {
            ViewBag.categorymid = id;
            ViewBag.categorymname = db.CategoryMiddles.Find(id).CategoryMName;

            TempData["oid"] = (Request.Cookies["oid"] == null) ? "1" : Request.Cookies["oid"].Values[0].ToString();

            ViewBag.categorysid = (sid != 0) ? sid : 0;
            ViewBag.categorysname = (sid != 0) ? db.CategorySmalls.Find(sid).CategorySName : null;

            return View();
        }

        //商品活動分類
        public ActionResult GetActivity(int id = 0)
        {
            var activities = db.Activities.Select(a => new { a.ActivityID, a.ActivityName });
            return Json(activities.ToList(), JsonRequestBehavior.AllowGet);
        }

        //商品小類別分類
        public ActionResult GetCategory(int id = 1)
        {
            var categories = db.CategorySmalls.Where(c => c.CategoryMID == id).GroupBy(c => c.CategorySID).Select(g => new { CategorySID = g.Key, g.FirstOrDefault().CategoryMID, g.FirstOrDefault().CategorySName, count = g.FirstOrDefault().Products.Count() });
            //var categories = db.CategorySmalls.GroupBy(c => c.CategorySID).Select(g => new { CategorySID = g.Key, g.FirstOrDefault().CategoryMID, g.FirstOrDefault().CategorySName, count = g.FirstOrDefault().Products.Count() });
            return Json(categories.ToList(), JsonRequestBehavior.AllowGet);

        }

        //塊狀商品展示
        public ActionResult GetProduct(int id = 1, int sid = 0, int orderid = 1, int page = 1)
        {
            IQueryable<ProductColor> preproducts = (sid != 0) ? db.ProductColors.Where(p => p.Product.CategorySID == sid) : db.ProductColors.Where(p => p.Product.CategorySmall.CategoryMID == id);

            var products = ProductMethod.CreateProductCells(preproducts);
            int pages = products.Count();

            var orderresult = ProductMethod.SetCellsByOrder(products, orderid).Skip((page - 1) * 16).Take(16);

            return Json(new { pages = pages, datas = orderresult.ToList() }, JsonRequestBehavior.AllowGet);

        }

        //判斷商品有無存在活動中
        public ActionResult GetSale(int id)
        {
            var sale = db.ActivityProducts.Where(a => a.ProductID == id).First().Activity.DiscountMethod.Discount;
            return Json(sale, JsonRequestBehavior.AllowGet);

        }

        //取得商品展示圖
        public ActionResult ByteImage(int id, int? colorid = 0)
        {
            if (colorid == 0)
            {
                colorid = db.ProductColors.Where(c => c.ProductID == id).Select(c => c.ColorID).First();
            }
            int? pid = db.ProductColors.Where(p => p.ProductID == id && p.ColorID == colorid).Select(p => p.PhotoID).First();
            byte[] by = db.Photos.Find(pid).Photo1;
            return File(by, "Image/jpeg");

        }

        //取得商品詳細資料
        public ActionResult GetProductDetail(int id = 1, int? colorid = 0)
        {
            //瀏覽過的紀錄
            if (Session["recent"] != null)
            {
                List<int> recent = (List<int>)Session["recent"];
                if (recent.Contains(id))
                {
                }
                else
                    recent.Add(id);
                Session["recent"] = recent;
            }
            else
            {
                List<int> recent = new List<int>();
                recent.Add(id);
                Session.Add("recent", recent);
            }

            var product = db.Products.Find(id);
            ProductDetail prodcutDisplay;

            ViewBag.categorymid = product.CategorySmall.CategoryMID;
            ViewBag.categorymname = product.CategorySmall.CategoryMiddle.CategoryMName;
            ViewBag.categorysname = product.CategorySmall.CategorySName;
            prodcutDisplay = new ProductDetail()
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Desctiption = product.Desctiption,
                CategorySID = product.CategorySID,
                UnitPrice = product.UnitPrice,
                WashingName = db.ProductWashings.Where(w => w.ProductID == id).Select(w => w.Washing.WashingName).ToArray(),
                Colors = db.ProductColors.Where(c => c.ProductID == id).AsEnumerable().Select(c => new Color { ColorID = c.ColorID, ColorName = c.Color.ColorName, R = c.Color.R, G = c.Color.G, B = c.Color.B }).ToList(),
                Sizes = db.ProductSizes.Where(s => s.ProductID == id).AsEnumerable().Select(s => new Size { SizeID = s.SizeID, SizeName = s.Size.SizeName }).ToList()
            };

            var inactivity = db.ActivityProducts.Where(a => a.ProductID == id).ToList();
            if (inactivity.Count() > 0)
            {
                ViewBag.activityname = inactivity.First().Activity.ActivityName;
                prodcutDisplay.SUnitPrice = Convert.ToInt32(Math.Floor(inactivity.First().Activity.DiscountMethod.Discount * product.UnitPrice));
            }

            ViewBag.colorid = (colorid == 0) ? db.ProductColors.Where(c => c.ProductID == id).Select(c => c.ColorID).First() : colorid;

            var images = db.ProductPhotoes.Where(p => p.ProductID == id).Take(10);
            List<string> imgs = new List<string>();
            foreach (var image in images)
            {
                var img = Convert.ToBase64String(image.Photo.Photo1);
                imgs.Add(img);
            }
            ViewBag.images = imgs;

            if (Session["IsLogin"] != null || Request.Cookies["IsLogin"] != null)
            {
                int uid = int.Parse(Request.Cookies["upid"].Value);
                var fav = db.MyFavorites.Where(f => f.UserID == uid && f.ProductID == id).ToList();

                ViewBag.fav = (fav.Count > 0) ? 1 : 0;
            }
            else
            {
                ViewBag.fav = 0;
            }

            var products = db.Products.Where(p => p.ProductID != id && p.CategorySID == prodcutDisplay.CategorySID).OrderBy(x => Guid.NewGuid()).Take(10).ToList();

            List<CartItem> citems = new List<CartItem>();
            CartItem citem;

            foreach (var pd in products)
            {
                decimal discountpercent = (db.ActivityProducts.Where(a => a.ProductID == pd.ProductID).Count() > 0) ? db.ActivityProducts.Where(a => a.ProductID == pd.ProductID).FirstOrDefault().Activity.DiscountMethod.Discount : 0;

                string activityname = (db.ActivityProducts.Where(a => a.ProductID == pd.ProductID).Count() > 0) ? db.ActivityProducts.Where(a => a.ProductID == pd.ProductID).FirstOrDefault().Activity.ActivityName : null;

                citem = new CartItem()
                {
                    ProductID = pd.ProductID,
                    ProductName = pd.ProductName,
                    UnitPrice = pd.UnitPrice,
                    SUnitPrice = Convert.ToInt32(Math.Floor(discountpercent * pd.UnitPrice)),
                    ActivityName = activityname
                };
                citems.Add(citem);
            }

            ViewBag.recommend = citems.ToList();

            var evaluation = ProductMethod.LoadEvaluation(id);
            if (evaluation.Count > 0)
            {
                int score = evaluation.Sum(e => e.Grade) / evaluation.Count;
                ViewBag.score = score;
            }
            ViewBag.evaluation = evaluation.ToList();

            return View(prodcutDisplay);

        }

        //查詢商品剩餘數量
        public ActionResult CheckQTY(int ProductID, int ColorID, int SizeID, int StockQTY)
        {
            var qty = db.ProductStocks.Where(s => s.ProductID == ProductID && s.ProductColor.ColorID == ColorID && s.ProductSize.SizeID == SizeID).FirstOrDefault().StockQTY;
            if (qty >= StockQTY)
                return Json("enough");
            else
                return Json("preorder");
        }

        //加入購物車
        public ActionResult AddCart(CartItem item)
        {
            List<CartItem> cart;

            item.ProductName = db.Products.Find(item.ProductID).ProductName;
            item.ColorName = db.Colors.Find(item.ColorID).ColorName;
            item.SizeName = db.Sizes.Find(item.SizeID).SizeName;

            //會員
            if (Session["IsLogin"] != null || Request.Cookies["IsLogin"] != null)
            {
                int uid = int.Parse(Request.Cookies["upid"].Value);
                int pcid = db.ProductColors.Where(c => c.ProductID == item.ProductID && c.ColorID == item.ColorID).First().ProductColorID;
                int psid = db.ProductSizes.Where(c => c.ProductID == item.ProductID && c.SizeID == item.SizeID).First().ProductSizeID;
                var sameitem = db.Carts.Where(c => c.UserID == uid && c.ProductID == item.ProductID && c.ProductColorID == pcid && c.ProductSizeID == psid).ToList();
                //購物車內已有相同款式商品
                if (sameitem.Count() > 0)
                {
                    sameitem.First().Quantity += item.OrderQTY;
                    db.Entry(sameitem.First()).State = System.Data.Entity.EntityState.Modified;
                }
                //購物車內無相同款式
                else
                {
                    Cart cartitem = new Cart()
                    {
                        UserID = uid,
                        ProductID = item.ProductID,
                        ProductColorID = pcid,
                        ProductSizeID = psid,
                        Quantity = item.OrderQTY,
                        UnitPrice = item.UnitPrice
                    };
                    db.Carts.Add(cartitem);
                }
                db.SaveChanges();
                //把db.cart內容回傳cart>
                cart = ProductMethod.Cart(uid);
                return Json(cart, JsonRequestBehavior.AllowGet);
            }
            //非會員
            else
            {
                ////已有購物車(非購買第一件商品)
                //if (Session["cart"] != null)
                //{
                //    cart = (List<CartItem>)Session["cart"];
                //    var incart = cart.Where(c => c.ProductID == item.ProductID && c.ColorID == item.ColorID && c.SizeID == item.SizeID).ToList();
                //    //購物車內已有相同款式商品
                //    if (incart.Count() > 0)
                //    {
                //        incart.First().OrderQTY += item.OrderQTY;
                //    }
                //    //購物車內無相同款式
                //    else
                //    {
                //        cart.Add(item);
                //    }
                //    Session.Add("cart", cart);
                //}
                ////尚未產生購物車(購買第一項商品)
                //else
                //{
                //    //建立購物車
                //    cart = new List<CartItem>();
                //    //加入商品
                //    cart.Add(item);
                //    Session.Add("cart", cart);
                //}
                return Json("notmember", JsonRequestBehavior.AllowGet);
            }

        }

        //載入購物車
        public ActionResult LoadCart()
        {
            //會員
            if (Session["IsLogin"] != null || Request.Cookies["IsLogin"] != null)
            {
                List<CartItem> cart;
                int uid = int.Parse(Request.Cookies["upid"].Value);
                var usercart = db.Carts.Where(c => c.UserID == uid);
                if (usercart.Count() > 0)
                {
                    //把db.cart內容回傳cart>
                    cart = ProductMethod.Cart(uid).ToList();
                    return Json(cart, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            //非會員
            else
            {
                if (Session["cart"] != null)
                {
                    return Json((List<CartItem>)Session["cart"], JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<CartItem> empty = new List<CartItem>();
                    return Json(empty, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //載入通知
        public ActionResult LoadCaution()
        {
            if (Session["IsLogin"] != null || Request.Cookies["IsLogin"] != null)
            {
                int uid = int.Parse(Request.Cookies["upid"].Value);
                var usernotice = db.UserNotices.Where(c => c.UserID == uid);
                if (usernotice.Count() > 0)
                {
                    var notice = usernotice.OrderByDescending(u => u.NoticeDate).AsEnumerable().Select(s => new { s.Comment, NoticeDate = s.NoticeDate.ToString("yyyy/MM/dd"), s.NoticeID, s.IsRead }).ToList();
                    var unread = usernotice.Where(u => u.IsRead == false).Count();
                    return Json(new { unread = unread, notice = notice }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { unread = 0, notice = "nocaution" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { unread = 0, notice = "nocaution" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReadByNoticeId(int id)
        {
            if (Session["IsLogin"] != null || Request.Cookies["IsLogin"] != null)
            {
                int uid = int.Parse(Request.Cookies["upid"].Value);
                var readstate = db.UserNotices.Find(id);
                readstate.IsRead = true;
                db.Entry(readstate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var unread = db.UserNotices.Where(u => u.UserID == uid && u.IsRead == false).Count();
                return Json(unread, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("notmember");
            }
        }

        //加入最愛
        public ActionResult AddFavorite(MyFavorite myFavorite)
        {
            if (Session["IsLogin"] != null || Request.Cookies["IsLogin"] != null)
            {
                int uid = int.Parse(Request.Cookies["upid"].Value);
                var fav = db.MyFavorites.Where(f => f.UserID == uid && f.ProductID == myFavorite.ProductID).ToList();
                string addstate;
                if (fav.Count() > 0)
                {
                    db.MyFavorites.Remove(fav.First());
                    addstate = "remove";
                }
                else
                {
                    myFavorite.UserID = uid;
                    db.MyFavorites.Add(myFavorite);
                    addstate = "add";
                }
                db.SaveChanges();
                return Json(addstate, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0);
            }
        }
    }
}