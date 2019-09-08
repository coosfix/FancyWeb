using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using FancyWeb.Areas.Backend.ViewModels;
using PagedList;
using PagedList.Mvc;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class AddStockController : Controller
    {
        private FancyStoreEntities db = new FancyStoreEntities();        

        // GET: Backend/AddStock
        public ActionResult StockList(int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            var prodStocks = db.ProductStocks.ToList();
            var orderDetails = db.OrderDetails.Where(x => x.OrderHeader.OrderStatusID == 1).ToList();
            var prodColors = db.ProductColors.ToList();
            var prodSizes = db.ProductSizes.ToList();
            var colors = db.Colors.ToList();
            var sizes = db.Sizes.ToList();
            var suppliers = db.Suppliers.ToList();
            var carts = db.Carts.ToList();
            var prods = db.Products.ToList();

            List<StockListViewModel> prodStockList = new List<StockListViewModel>();

            foreach (var item in prodStocks)
            {
                StockListViewModel stk = new StockListViewModel();
                stk.StockID = item.StockID;
                stk.ProductColorID = item.ProductColorID;
                stk.ProductSizeID = item.ProductSizeID;
                stk.MinStock = item.MinStock;
                stk.StockQTY = item.StockQTY;

                stk.OrderQTY = orderDetails
                    .Where(x => x.ProductID == item.ProductID && x.ProductColorID == item.ProductColorID
                                  && x.ProductSizeID == item.ProductSizeID).Select(x => x.OrderQTY).Count();

                stk.CartQTY = carts.Where(x => x.ProductID == item.ProductID && x.ProductColorID == item.ProductColorID
                                  && x.ProductSizeID == item.ProductSizeID).Select(x => x.Quantity).Count();

                stk.AddQTY = stk.CartQTY + stk.OrderQTY + stk.MinStock - stk.StockQTY;

                var p = prods.Where(x=> x.ProductID == item.ProductID).FirstOrDefault();
                stk.ProductName = p.ProductName;

                var s = suppliers.Where(x=>x.SupplierID==(int)p.SupplierID).FirstOrDefault();
                stk.SupplierName = s.SupplierName;
                stk.SupplierID = s.SupplierID;

                var pc = prodColors.Where(x=>x.ProductColorID==item.ProductColorID).FirstOrDefault();
                stk.ProductColor = colors.Where(x=>x.ColorID==pc.ColorID).FirstOrDefault().ColorName;

                if ((pc.PhotoID == null ? 0 : pc.PhotoID) > 0)
                {
                    stk.PhotoID = (int)pc.PhotoID;
                }

                var ps = prodSizes.Where(x=>x.ProductSizeID==item.ProductSizeID).FirstOrDefault();
                stk.ProductSize = sizes.Where(x=>x.SizeID==ps.SizeID).FirstOrDefault().SizeName;

                if (stk.AddQTY > 0)
                {
                    prodStockList.Add(stk);
                }
            }

            return View(prodStockList.OrderByDescending(p => p.SupplierID).ToPagedList(page ?? 1, 5));
        }

        /// <summary>
        /// //更新補貨作業
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>        
        [HttpGet]
        public ActionResult Edit(int id = 1)
        {
            StockListViewModel stk = new StockListViewModel();
            var prodStocks = db.ProductStocks.Where(x=>x.StockID==id).FirstOrDefault();

            stk.StockID = prodStocks.StockID;
            stk.ProductID = prodStocks.ProductID;
            stk.ProductColorID = prodStocks.ProductColorID;
            stk.ProductSizeID = prodStocks.ProductSizeID;
            stk.StockQTY = prodStocks.StockQTY;
            stk.MinStock = prodStocks.MinStock;

            var p = db.Products.Where(x=>x.ProductID==prodStocks.ProductID).FirstOrDefault();
            stk.ProductName = p.ProductName;

            var s = db.Suppliers.Where(x=>x.SupplierID==(int)p.SupplierID).FirstOrDefault();
            stk.SupplierName = s.SupplierName;

            var pc = db.ProductColors.Where(x=>x.ProductColorID==prodStocks.ProductColorID).FirstOrDefault();
            stk.ProductColor = db.Colors.Where(x=>x.ColorID==pc.ColorID).FirstOrDefault().ColorName;

            var ps = db.ProductSizes.Where(x=>x.ProductSizeID==prodStocks.ProductSizeID).FirstOrDefault();
            stk.ProductSize = db.Sizes.Where(x=>x.SizeID==ps.SizeID).FirstOrDefault().SizeName;


            stk.OrderQTY = db.OrderDetails
                .Where(x => x.ProductID == prodStocks.ProductID && x.ProductColorID == prodStocks.ProductColorID
                              && x.ProductSizeID == prodStocks.ProductSizeID && x.OrderHeader.OrderStatusID == 1).Select(x => x.OrderQTY).Count();

            stk.CartQTY = db.Carts.Where(x => x.ProductID == prodStocks.ProductID && x.ProductColorID == prodStocks.ProductColorID
                              && x.ProductSizeID == prodStocks.ProductSizeID).Select(x => x.Quantity).Count();

            stk.AddQTY = stk.CartQTY + stk.OrderQTY + stk.MinStock - stk.StockQTY;

            return View(stk);
        }

        [HttpPost]
        public ActionResult Edit(StockListViewModel sl)
        {
            ProductStock ps = db.ProductStocks.Where(x=>x.StockID==sl.StockID).FirstOrDefault();

            ps.StockQTY = ps.StockQTY + sl.AddQTY;

            db.Entry(ps).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("StockList", "AddStock", new { area = "Backend", page = TempData["savepage"] });
        }

        //讀DB 二進位資料
        public ActionResult GetImageByte(int id = 1)
        {
            Photo ph = db.Photos.Find(id);
            byte[] img = ph.Photo1;

            return File(img, "image/jpeg");
        }

        //讀取Supplier
        public ActionResult SupplierJson(int spid)
        {
            var splist = db.Suppliers.Where(x => x.SupplierID == spid).Select(x => new
            {
                x.Phone,
                x.Fax,
                x.Email,
                x.Address
            }).FirstOrDefault();

            return Json(splist, JsonRequestBehavior.AllowGet);
        }
    }
}