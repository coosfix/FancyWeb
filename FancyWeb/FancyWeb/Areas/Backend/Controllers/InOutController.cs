using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using FancyWeb.Models;
using FancyWeb.Areas.Backend.ViewModels;
using PagedList;
using PagedList.Mvc;

namespace FancyWeb.Areas.Backend.Controllers
{
    public class InOutController : Controller
    {
        private FancyStoreEntities db = new FancyStoreEntities();

        /// <summary>
        /// 產品資料
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        // GET: Backend/InOut
        public ActionResult Product(int? page)
        {
            //記錄目前頁數,若是空值就給1
            TempData["savepage"] = page ?? 1;

            var prod = db.Products.ToList();
            var cateMS = db.VW_EW_CategorySM.ToList();
            var supplier = db.Suppliers.ToList();
            var pc = db.ProductColors.ToList();
            List<ProductViewModel> prodList = new List<ProductViewModel>();

            foreach (var item in prod)
            {
                ProductViewModel pd = new ProductViewModel();

                pd.ProductID = item.ProductID;
                pd.ProductName = item.ProductName;
                pd.Desctiption = item.Desctiption;
                pd.CategorySID = item.CategorySID;
                pd.SupplierID = item.SupplierID;
                pd.UnitPrice = item.UnitPrice;
                pd.ProductInDate = item.ProductInDate;
                pd.ProductOutDate = item.ProductOutDate;
                pd.CreateDate = item.CreateDate;


                if (pc.Any(x => x.ProductID == item.ProductID && ((x.PhotoID == null ? 0 : x.PhotoID) > 0)))
                {
                    pd.PhotoID = (int)pc.Where(x => x.ProductID == item.ProductID).FirstOrDefault().PhotoID;
                }


                var c = cateMS.Where(x => x.CategorySID == item.CategorySID).FirstOrDefault();
                pd.CategoryMSName = c.CategoryName;

                var s = supplier.Where(x => x.SupplierID == item.SupplierID).FirstOrDefault();
                pd.SupplierName = s.SupplierName;

                prodList.Add(pd);
            }

            return View(prodList.ToPagedList(page ?? 1, 5));
        }

        /// <summary>
        /// //更新上下架
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>        
        [HttpGet]
        public ActionResult Edit(int id = 1)
        {
            ProductViewModel pd = new ProductViewModel();
            var prod = db.Products.Find(id);
            var cateMS = db.VW_EW_CategorySM.ToList();
            var supplier = db.Suppliers.ToList();

            pd.ProductID = prod.ProductID;
            pd.ProductName = prod.ProductName;
            pd.Desctiption = prod.Desctiption;
            pd.CategorySID = prod.CategorySID;
            pd.SupplierID = prod.SupplierID;
            pd.UnitPrice = prod.UnitPrice;
            pd.ProductInDate = prod.ProductInDate;
            pd.ProductOutDate = prod.ProductOutDate;
            pd.CreateDate = prod.CreateDate;

            var c = cateMS.Where(x => x.CategorySID == prod.CategorySID).FirstOrDefault();
            pd.CategoryMSName = c.CategoryName;

            var s = supplier.Where(x => x.SupplierID == prod.SupplierID).FirstOrDefault();
            pd.SupplierName = s.SupplierName;

            return View(pd);
        }

        [HttpPost]
        public ActionResult Edit(ProductViewModel pd)
        {
            var prod = db.Products.Find(pd.ProductID);

            prod.ProductInDate = pd.ProductInDate;
            prod.ProductOutDate = pd.ProductOutDate;

            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Product", "InOut", new { area = "Backend", page = TempData["savepage"] });
        }

        /// <summary>
        /// //檔案上傳
        /// </summary>
        /// <returns></returns>        
        [HttpGet]
        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase FileInfo)
        {
            ViewBag.Message = "";

            if (FileInfo != null)
            {
                //將檔案上傳至 App_Data\Upload 目錄中
                string folder = Server.MapPath(@"~/App_Data/Upload/");

                if (!Directory.Exists(folder))
                {
                    //建立資料夾
                    Directory.CreateDirectory(folder);
                }

                string savefilename = Path.Combine(folder, FileInfo.FileName);
                FileInfo.SaveAs(savefilename);

                //檢查檔案類型是否為excel
                var fileclass = "";
                for (int i = 0; i < 2; i++)
                {
                    fileclass += FileInfo.InputStream.ReadByte().ToString();
                }

                //8075:.xlsx,  208207:.xls
                bool isExcel = (fileclass == "8075" || fileclass == "208207");

                if (isExcel)
                {
                    //Excel 介面
                    IWorkbook workbook;

                    if (fileclass == "208207")
                    {
                        //讀取專案內中的excel 檔案
                        using (FileStream file = new FileStream(savefilename, FileMode.Open, FileAccess.Read))
                        {
                            workbook = new HSSFWorkbook(file);
                        }
                    }
                    else
                    {
                        //讀取專案內中的excel 檔案
                        using (FileStream file = new FileStream(savefilename, FileMode.Open, FileAccess.Read))
                        {
                            workbook = new XSSFWorkbook(file);
                        }
                    }

                    //讀取Sheet1 工作表
                    var sheet = workbook.GetSheetAt(0);

                    for (int row = 1; row <= sheet.LastRowNum; row++)//從第2列開始
                    {
                        if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                        {
                            var prod = new Product();

                            if (GetCellValue(sheet.GetRow(row), 0) != "")
                            {
                                //若存在ProductID才要更新資料
                                //int pid = Convert.ToInt32(GetCellValue(sheet.GetRow(row), 0));
                                int pid = int.Parse(GetCellValue(sheet.GetRow(row), 0));
                                bool hasProduct = db.Products.Any(p => p.ProductID == pid);
                                if (hasProduct)
                                {
                                    prod = db.Products.Find(pid);
                                    prod.ProductInDate = DateTime.Parse(GetCellValue(sheet.GetRow(row), 5));
                                    prod.ProductOutDate = DateTime.Parse(GetCellValue(sheet.GetRow(row), 6));

                                    db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();

                                    Response.Cookies["mesg"].Value = GetCellValue(sheet.GetRow(row), 5);
                                }
                            }
                        }
                    }

                    return RedirectToAction("Product", "InOut", new { area = "Backend" });
                }
                else
                {
                    ViewBag.Message = "上傳檔案不是Excel格式, 請重新選擇匯入的檔案";
                }
            }
            else
            {
                ViewBag.Message = "請選擇匯入的檔案";
            }
            return View();
        }

        /// <summary>
        /// 匯入-判別欄位型態
        /// </summary>
        /// <param name="iRow"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        private string GetCellValue(IRow iRow, int cellIndex)
        {
            var cell = iRow.ElementAtOrDefault(cellIndex);
            string columnStr = string.Empty;

            if (cell == null)
            {
                return "";
            }
            else
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:  // 數值格式
                        if (DateUtil.IsCellDateFormatted(cell))
                        {   // 日期格式
                            columnStr = cell.DateCellValue.ToString();
                        }
                        else
                        {   // 數值格式
                            columnStr = cell.NumericCellValue.ToString();
                        }
                        break;
                    case CellType.String:   // 字串格式
                        columnStr = cell.StringCellValue;
                        break;
                    default:
                        {
                            columnStr = cell.StringCellValue;
                            break;
                        }
                }
            }
            return columnStr;
        }

        /// <summary>
        /// //檔案下載
        /// </summary>
        /// <returns></returns>   
        public ActionResult Export()
        {
            //Response.Cookies["mesg"].Value = "export post";

            //建立Excel檔案的物件
            XSSFWorkbook workbook = new XSSFWorkbook();
            //新增一個sheet
            ISheet sheet1 = workbook.CreateSheet("Sheet1");

            XSSFCellStyle headStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            XSSFFont font = (XSSFFont)workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.Boldweight = 700;
            headStyle.SetFont(font);

            //給sheet1新增第一行的頭部標題列
            IRow headerrow = sheet1.CreateRow(0);
            for (int i = 0; i < 7; i++)
            {
                ICell cell = headerrow.CreateCell(i);
                cell.CellStyle = headStyle;
            }
            //給欄位表頭名稱
            headerrow.CreateCell(0).SetCellValue("商品編號");
            headerrow.CreateCell(1).SetCellValue("商品名稱");
            headerrow.CreateCell(2).SetCellValue("分類");
            headerrow.CreateCell(3).SetCellValue("單價");
            headerrow.CreateCell(4).SetCellValue("供應商");
            headerrow.CreateCell(5).SetCellValue("上架日期");
            headerrow.CreateCell(6).SetCellValue("下架日期");

            var products = db.Products.ToList();
            var cateMS = db.VW_EW_CategorySM.ToList();
            var supplier = db.Suppliers.ToList();

            int k = 1;
            foreach (var prod in products)
            {
                IRow rowtemp = sheet1.CreateRow(k);

                var c = cateMS.Where(x => x.CategorySID == prod.CategorySID).FirstOrDefault();
                var s = supplier.Where(x => x.SupplierID == prod.SupplierID).FirstOrDefault();

                rowtemp.CreateCell(0).SetCellValue(prod.ProductID.ToString());
                rowtemp.CreateCell(1).SetCellValue(prod.ProductName);
                rowtemp.CreateCell(2).SetCellValue(c.CategoryName);
                rowtemp.CreateCell(3).SetCellValue($"{prod.UnitPrice:c0}");
                rowtemp.CreateCell(4).SetCellValue(s.SupplierName);
                rowtemp.CreateCell(5).SetCellValue(((DateTime)prod.ProductInDate).ToString("yyyy/MM/dd"));
                rowtemp.CreateCell(6).SetCellValue(((DateTime)prod.ProductOutDate).ToString("yyyy/MM/dd"));
                k++;
            }

            // 寫入到客戶端 
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Close();

            return File(ms.ToArray(), "application/vnd.ms-excel", "商品上下架日期.xlsx");
        }
    }
}