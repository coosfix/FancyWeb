using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using FancyWeb.Areas.Rating.ViewModel;

namespace FancyWeb.Areas.Rating.Controllers
{
    public class RatingController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();
        // GET: Rating/Rating
        public ActionResult Index()
        {
            int userAccount = Int32.Parse(Request.Cookies["upid"].Value);
            //var userAccount = 1;
            List<RatingProductViewModel> rplist = new List<RatingProductViewModel>();

            //出貨狀態為已出貨的訂單商品
            var shipped_orderList = db.OrderDetails.Where(o => o.OrderHeader.UserID == userAccount && o.OrderHeader.OrderStatusID == 2)
                                                          .Select(o => new
                                                          {
                                                              productID = o.ProductID,
                                                              productname = o.Product.ProductName,
                                                              productcolor = o.ProductColor.Color.ColorName,
                                                              productsize = o.ProductSize.Size.SizeName,
                                                              photoID = o.ProductColor.PhotoID,
                                                              userID = o.OrderHeader.UserID,
                                                              createdate = o.CreateDate,
                                                              ordernumber = o.OrderHeader.OrderNum
                                                          }).ToList();

            //評價table中該user已經新增的商品編號
            var rate_prodList = db.ProductEvaluations.Where(p => p.UserID == userAccount).Select(p => p.ProductID).ToList();

            //評價table中該user已經新增的訂單號碼
            var rate_OrderList = db.ProductEvaluations.Where(p => p.UserID == userAccount).Select(p => p.OrderNum).ToList();


            foreach (var item in shipped_orderList)
            {
                //如果評價table中已經有此訂單號碼,就再判斷評價table有沒有該商品編號
                //已評價=如果評價table中有此訂單號碼,也有此商品編號就顯示
                if (rate_OrderList.Contains(item.ordernumber))
                {
                    if (!rate_prodList.Contains(item.productID))
                    {
                        RatingProductViewModel rp = new RatingProductViewModel();
                        rp.OrderNum = item.ordernumber;
                        rp.ProductName = item.productname;
                        rp.ColorName = item.productcolor;
                        rp.SizeName = item.productsize;
                        rp.PhotoID = item.photoID;
                        rp.ProductID = item.productID;
                        rp.UserID = item.userID;
                        rp.CreateDate = item.createdate;
                        rplist.Add(rp);
                    }
                }
                //如果評價table中沒有此訂單號碼就應該顯示
                else
                {
                    RatingProductViewModel rp = new RatingProductViewModel();
                    rp.OrderNum = item.ordernumber;
                    rp.ProductName = item.productname;
                    rp.ColorName = item.productcolor;
                    rp.SizeName = item.productsize;
                    rp.PhotoID = item.photoID;
                    rp.ProductID = item.productID;
                    rp.UserID = item.userID;
                    rp.CreateDate = item.createdate;
                    rplist.Add(rp);
                }
            }
            return View(rplist.ToList());
        }


        public ActionResult GetImageByte(int id = 1)
        {
            Photo ph = db.Photos.Find(id);
            byte[] img = ph.Photo1;
            return File(img, "image/jpeg");
        }


        [HttpPost]
        public ActionResult SaveComment(FormCollection post)
        {
            ProductEvaluation pe = new ProductEvaluation();
            pe.ProductID = Convert.ToInt32(post["pid"]);
            pe.UserID = Int32.Parse(Request.Cookies["upid"].Value);  //16;
            pe.Grade = Convert.ToInt32(post["grade"]);
            pe.Comment = post["comment"];
            pe.EvaluationDate = DateTime.Now;
            pe.OrderNum = post["ordernumber"];

            try
            {
                db.ProductEvaluations.Add(pe);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                return Json(
                new
                {
                    Status = false,
                    Message = ex.ToString()
                });

            }

            return Json(
                new
                {
                    Status = true,
                });
        }

        //public ActionResult Comment()
        //{

        //    return View();
        //}

        //[HttpGet]
        //public ActionResult GetAverageRate(int prodid)
        //{
        //    var rateList = db.ProductEvaluations.Where(p => p.ProductID == prodid).Join(db.Users, p => p.UserID, u => u.UserID, (p, u) => new
        //    {
        //        username = u.UserName,
        //        prodid = p.ProductID,
        //        rate = p.Grade,
        //        comment = p.Comment,
        //        lastUpdateTime = p.EvaluationDate.ToShortTimeString()
        //    }).OrderByDescending(cs => cs.lastUpdateTime).ToList();

        //    double total = rateList.Select(c => c.rate).Sum();
        //    double count = rateList.Count;
        //    var average = Math.Round(total / count);

        //    return null; 
        //}

    };
};