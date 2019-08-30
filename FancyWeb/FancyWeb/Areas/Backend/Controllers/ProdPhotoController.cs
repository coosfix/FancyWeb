using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FancyWeb.Models;
using FancyWeb.Areas.Backend.ViewModels;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class ProdPhotoController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();

        // GET: Backend/ProdPhoto
        public ActionResult Index(int id)
        {
            ProductPhotoVIewModel pcvm = new ProductPhotoVIewModel();

            var pd = db.Products.Find(id);

            pcvm.ProductID = pd.ProductID;
            pcvm.ProductName = pd.ProductName;

            return View(pcvm);
        }

        public ActionResult ProdPhotoJson(int id)
        {
            List<ProductPhotoVIewModel> pcvmlist = new List<ProductPhotoVIewModel>();

            var ps = db.ProductPhotoes.Where(x => x.ProductID == id).OrderByDescending(x => x.ProductPhotoID);
            var c = db.Colors;

            foreach (var item in ps)
            {
                ProductPhotoVIewModel psvm = new ProductPhotoVIewModel
                {
                    ProductPhotoID = item.ProductPhotoID,
                    ProductID = item.ProductID,
                    CreateDate = item.CreateDate,
                    PhotoID = (int)(item.PhotoID == null ? 0 : item.PhotoID),
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
        public ActionResult Create(int pid, int phid)
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

                ProductPhoto pc = new ProductPhoto
                {
                    ProductID = pid,
                    PhotoID = phid,
                    CreateDate = DateTime.Now
                };

                db.ProductPhotoes.Add(pc);
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

                ProductPhoto pc = db.ProductPhotoes.Find(pcid);

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
            try
            {
                var pp = db.ProductPhotoes.Find(pcid);

                //db.Photos.Remove(db.Photos.Find(pp.PhotoID));

                db.ProductPhotoes.Remove(pp);
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

