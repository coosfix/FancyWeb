using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using FancyWeb.Areas.Backend.ViewModels;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class ProdColorController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/ProdColor
        public ActionResult Index(int id)
        {
            ProductColorVIewModel pcvm = new ProductColorVIewModel();

            var pd = db.Products.Find(id);

            pcvm.ProductID = pd.ProductID;
            pcvm.ProductName = pd.ProductName;
            ViewBag.productcolor = db.Colors;

            return View(pcvm);
        }

        public ActionResult ProdColorJson(int id)
        {
            List<ProductColorVIewModel> pcvmlist = new List<ProductColorVIewModel>();

            var ps = db.ProductColors.Where(x => x.ProductID == id).OrderByDescending(x => x.ProductColorID);
            var c = db.Colors;

            foreach (var item in ps)
            {
                ProductColorVIewModel psvm = new ProductColorVIewModel
                {
                    ProductColorID = item.ProductColorID,
                    ProductID = item.ProductID,
                    ColorID = item.ColorID,
                    CreateDate = item.CreateDate,
                    ColorName = c.Where(x => x.ColorID == item.ColorID).FirstOrDefault().ColorName,
                    PhotoID = (item.PhotoID == null ? 0 : item.PhotoID),
                };

                pcvmlist.Add(psvm);
            }

            return Json(pcvmlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadFile(string Parm)
        {
            HttpPostedFileBase fileBase = Request.Files[0];
            var phid = 0;

            try
            {
                //將圖檔轉成2進位資料
                var imgSize = fileBase.ContentLength;
                byte[] imgByte = new byte[imgSize];
                fileBase.InputStream.Read(imgByte, 0, imgSize);

                Photo ph = new Photo
                {
                    Photo1 = imgByte,
                    CreateDate = DateTime.Now
                };

                db.Photos.Add(ph);
                db.SaveChanges();

                phid = ph.PhotoID;
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = false,
                    photoid = phid,
                    Message = ex.ToString(),
                });
            }

            return Json(
                new
                {
                    Status = true,
                    photoid = phid,
                    Message = "上傳成功=> " + phid
                });
        }

        [HttpPost]
        public ActionResult Create(int pid, int cid, int phid)
        {
            if (db.ProductColors.Any(x => x.ProductID == pid && x.ColorID == cid))
            {
                return Json(new
                {
                    Status = false,
                    Message = "顏色重覆選取 !",
                });
            }

            if (phid == 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "圖檔未選取 !",
                });
            }
            try
            {

                ProductColor pc = new ProductColor
                {
                    ProductID = pid,
                    ColorID = cid,
                    PhotoID = phid,
                    CreateDate = DateTime.Now
                };

                db.ProductColors.Add(pc);
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
                photoid = phid
            });
        }

        [HttpPost]
        public ActionResult Edit(int pcid, int phid)
        {
            if (phid == 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "圖檔未選取 !",
                });
            }
            try
            {

                ProductColor pc = db.ProductColors.Find(pcid);

                pc.PhotoID = phid;

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
                photoid = phid
            });
        }

        //刪除
        public ActionResult Delete(int pcid)
        {
            if (db.ProductStocks.Any(x => x.ProductColorID == pcid))
            {
                return Json(new
                {
                    Status = false,
                    Message = "尚有庫存資料, 不能刪除 !",
                });
            }

            try
            {
                var pc = db.ProductColors.Find(pcid);

                //db.Photos.Remove(db.Photos.Find(pc.PhotoID));

                db.ProductColors.Remove(pc);
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