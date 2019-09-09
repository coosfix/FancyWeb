using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using PagedList;
using PagedList.Mvc;
using FancyWeb.Areas.Backend.ViewModels;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class ShipmentController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/Shipment
        public ActionResult Index(int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            List<ShipHeaderViewModel> shipList = new List<ShipHeaderViewModel>();
            var oh = db.OrderHeaders.Where(x => x.OrderStatusID == 1).OrderBy(x => x.OrderDate)
                         .Select(x => new
                         {
                             x.OrderID,
                             x.OrderNum,
                             x.OrderDate,
                             x.OrderStatusID,
                             x.OrderStatusList.OrderStatusName,
                             x.PayMethodID,
                             x.PayMethod.PayMethodName,
                             x.ShippingID,
                             x.Shipping.ShippingName
                         });

            var od = db.VW_EW_OrderDetail.ToList();

            foreach (var item in oh)
            {
                ShipHeaderViewModel shv = new ShipHeaderViewModel();
                shv.OrderID = item.OrderID;
                shv.OrderNum = item.OrderNum;
                shv.OrderDate = item.OrderDate;
                shv.OrderStatusID = item.OrderStatusID;
                shv.OrderStatusName = item.OrderStatusName;
                shv.PayMethodID = item.PayMethodID;
                shv.PayMethodName = item.PayMethodName;
                shv.ShippingID = item.ShippingID;
                shv.ShippingName = item.ShippingName;
                if (od.Where(x => x.OrderID == item.OrderID).Any(x => x.Balance < 0))
                { shv.CanShip = false; }
                else
                { shv.CanShip = true; }

                shipList.Add(shv);
            }

            return View(shipList.ToPagedList(page ?? 1, 5));
        }

        public ActionResult Detail(int id, int page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page;

            var od = db.OrderDetails.Where(x => x.OrderID == id).ToList();
            var st = db.ProductStocks.ToList();
            int i = 0;

            List<ShipDetailViewModel> odList = new List<ShipDetailViewModel>();
            foreach (var item in od)
            {
                ShipDetailViewModel sdv = new ShipDetailViewModel();
                sdv.ProductID = item.ProductID;
                sdv.ProductColorID = item.ProductColorID;
                sdv.ProductSizeID = item.ProductSizeID;
                sdv.ProductName = item.Product.ProductName;
                sdv.ProductColor = item.ProductColor.Color.ColorName;
                sdv.ProductSize = item.ProductSize.Size.SizeName;

                if ((item.ProductColor.PhotoID == null ? 0 : item.ProductColor.PhotoID) > 0)
                {
                    sdv.PhotoID = (int)item.ProductColor.PhotoID;
                }
                sdv.OrderQTY = item.OrderQTY;
                var s = st.Where(x => x.ProductID == item.ProductID && x.ProductColorID == item.ProductColorID && x.ProductSizeID == item.ProductSizeID).FirstOrDefault();
                sdv.StockID = s.StockID;
                sdv.StockQTY = s.StockQTY;
                sdv.BalanceQTY = sdv.StockQTY - sdv.OrderQTY;
                if (sdv.BalanceQTY < 0)
                { i++; }

                odList.Add(sdv);
            }

            ShipmaneAllViewModel sa = new ShipmaneAllViewModel();
            sa.shipdetailList = odList;
            ShipHeaderViewModel shv = new ShipHeaderViewModel();
            var oh = db.OrderHeaders.Where(x => x.OrderID == id).FirstOrDefault();
            shv.OrderID = (int)oh.OrderID;
            sa.OrderID = (int)oh.OrderID;
            shv.OrderNum = oh.OrderNum;
            shv.OrderDate = oh.OrderDate;
            shv.OrderStatusID = oh.OrderStatusID;
            shv.OrderStatusName = oh.OrderStatusList.OrderStatusName;
            shv.PayMethodID = oh.PayMethodID;
            shv.PayMethodName = oh.PayMethod.PayMethodName;
            shv.ShippingID = oh.ShippingID;
            sa.ShippingID = oh.ShippingID;
            shv.ShippingName = oh.Shipping.ShippingName;
            if (i > 0)
            { shv.CanShip = false; }
            else
            { shv.CanShip = true; }

            sa.shipheader = shv;
            ViewBag.datas = db.Shippings.ToList();

            return View(sa);
        }

        [HttpPost]
        public ActionResult Edit(ShipmaneAllViewModel sa, int page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page;

            var oh = db.OrderHeaders.Find(sa.OrderID);
            oh.ShippingID = sa.ShippingID;

            db.Entry(oh).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { page = TempData["savepage"] });
        }

        [HttpPost]
        public ActionResult Ship(int orderid)
        {
            try
            {
                var od = db.VW_EW_OrderDetail.Where(x => x.OrderID == orderid);

                foreach (var item in od)
                {
                    var st = db.ProductStocks.Where(x => x.ProductID == item.ProductID && x.ProductColorID == item.ProductColorID
                    && x.ProductSizeID == item.ProductSizeID).FirstOrDefault();

                    st.StockQTY = (int)item.Balance;
                    db.Entry(st).State = System.Data.Entity.EntityState.Modified;
                }
                var oh = db.OrderHeaders.Find(orderid);
                oh.OrderStatusID = 2;
                oh.ShipDate = DateTime.Now;
                db.Entry(oh).State = System.Data.Entity.EntityState.Modified;

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