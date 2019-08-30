using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using FancyWeb.Areas.Backend.ViewModels;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class ProdStockController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/ProdStock
        public ActionResult Index(int id)
        {
            ProductStockVIewModel psvm = new ProductStockVIewModel();

            var pd = db.Products.Find(id);

            psvm.ProductID = pd.ProductID;
            psvm.ProductName = pd.ProductName;

            ViewBag.productcolor = db.ProductColors.Where(x=>x.ProductID ==id).Select(x=>new
            {
                x.ProductColorID,
                x.Color.ColorName
            });

            ViewBag.productsize = db.ProductSizes.Where(x => x.ProductID == id).Select(x => new
            {
                x.ProductSizeID,
                x.Size.SizeName
            });

            return View(psvm);
        }

        public ActionResult ProdStockJson(int id)
        {
            List<ProductStockVIewModel> psvmlist = new List<ProductStockVIewModel>();

            var ps = db.ProductStocks.Where(x => x.ProductID == id).OrderByDescending(x => x.StockID);
            var c = db.ProductColors.Where(x => x.ProductID == id).Select(x => new
                        {
                            x.ProductColorID,
                            x.Color.ColorName
                        });
            var s = db.ProductSizes.Where(x => x.ProductID == id).Select(x => new
                        {
                            x.ProductSizeID,
                            x.Size.SizeName
                        });

            foreach (var item in ps)
            {
                ProductStockVIewModel psvm = new ProductStockVIewModel
                {
                    StockID = item.StockID,
                    ProductID = item.ProductID,
                    ProductColorID = item.ProductColorID,
                    ProductSizeID = item.ProductSizeID,
                    StockQTY = item.StockQTY,
                    MinStock = item.MinStock,
                    CreateDate = item.CreateDate,
                    PhotoID = item.ProductColor.PhotoID,

                    ProductColor = c.Where(x => x.ProductColorID == item.ProductColorID).FirstOrDefault().ColorName,
                    ProductSize = s.Where(x => x.ProductSizeID == item.ProductSizeID).FirstOrDefault().SizeName
                };

                psvmlist.Add(psvm);
            }

            return Json(psvmlist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(int pid, int sid, int cid, int stockqty, int minqty)
        {
            try
            {
                if (db.ProductStocks.Any(x => x.ProductID == pid && x.ProductSizeID == sid && x.ProductColorID==cid))
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "庫存資料已存在 !",
                    });
                }

                ProductStock ps = new ProductStock
                {
                    ProductID = pid,
                    ProductColorID = cid,
                    ProductSizeID = sid,
                    StockQTY = stockqty,
                    MinStock = minqty,
                    CreateDate = DateTime.Now
                };

                db.ProductStocks.Add(ps);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = false,
                    Message = ex.ToString(),
                });
            }

            return Json(new
            {
                Status = true,
            });
        }

        [HttpPost]
        public ActionResult Edit(int psid, int stockqty, int minqty)
        {
            try
            {

                ProductStock pc = db.ProductStocks.Find(psid);

                pc.StockQTY = stockqty;
                pc.MinStock = minqty;

                db.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = false,
                    Message = ex.ToString(),
                });
            }

            return Json(new
            {
                Status = true,
            });
        }

        //刪除
        public ActionResult Delete(int psid)
        {
            var ps = db.ProductStocks.Find(psid);

            if(db.OrderDetails.Any(x=>x.ProductID==ps.ProductID&&x.ProductColorID==ps.ProductColorID&&x.ProductSizeID==ps.ProductSizeID))
            {
                return Json(new
                {
                    Status = false,
                    Message = "訂單有資料, 不能刪除 !",
                });
            }

            if (db.Carts.Any(x => x.ProductID == ps.ProductID && x.ProductColorID == ps.ProductColorID && x.ProductSizeID == ps.ProductSizeID))
            {
                return Json(new
                {
                    Status = false,
                    Message = "購物車有資料, 不能刪除 !",
                });
            }

            try
            {
                db.ProductStocks.Remove(ps);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = false,
                    Message = ex.ToString(),
                });
            }

            return Json(new
            {
                Status = true,
            });
        }
    }
}