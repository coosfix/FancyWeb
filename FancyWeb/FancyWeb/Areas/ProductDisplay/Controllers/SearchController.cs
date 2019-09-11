using FancyWeb.Areas.ProductDisplay.Functions;
using FancyWeb.Areas.ProductDisplay.Models;
using FancyWeb.Areas.ProductDisplay.ViewModels;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.ProductDisplay.Controllers
{
    public class SearchController : Controller
    {
        FancyStoreEntities db = new FancyStoreEntities();
        // GET: ProductDisplay/Search
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Result(string keyword)
        {
            ViewBag.keyword = keyword;
            return View();
        }

        public ActionResult CategoryFilter()
        {
            List<CategoryFilter> searchFilters = new List<CategoryFilter>();
            CategoryFilter categoryFilter;
            var categories = db.CategoryMiddles.ToList();
            foreach (var category in categories)
            {
                categoryFilter = new CategoryFilter();
                categoryFilter.FilterName = category.CategoryMName;
                Dictionary<string, int> dicscategory = new Dictionary<string, int>();
                var scategories = db.CategorySmalls.Where(s => s.CategoryMID == category.CategoryMID).ToList();
                foreach (var scategory in scategories)
                {
                    dicscategory.Add(scategory.CategorySName, scategory.CategorySID);
                }
                categoryFilter.Filter = dicscategory;
                searchFilters.Add(categoryFilter);
            }
            return PartialView(searchFilters);
        }

        public ActionResult ColorFilter()
        {
            CategoryFilter colorFilter = new CategoryFilter();
            var colors = db.Colors.ToList();
            return PartialView(colors);
        }

        public ActionResult SizeFilter()
        {
            CategoryFilter sizeFilter = new CategoryFilter();
            var sizes = db.Sizes.AsEnumerable().Reverse().ToList();
            return PartialView(sizes);
        }

        public ActionResult ActivityFilter()
        {
            CategoryFilter activityFilter = new CategoryFilter();
            var activities = db.Activities.Where(a => a.EndDate >= DateTime.Now).ToList();
            return PartialView(activities);
        }

        public ActionResult GetProduct(SearchFilters searchFilters, int page = 1)
        {
            IEnumerable<ProductCell> result = SearchMethod.ClassifyResult(searchFilters).AsEnumerable();
            int pages = result.Count();

            if (pages > 0)
                return Json(new { pages = pages, datas = result.Skip((page - 1) * 16).Take(16).ToList() }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { pages = 1, datas = "noresult" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetKeyWord()
        {
            var keywords = db.KeyWords.Take(6).ToList();
            return Json(keywords, JsonRequestBehavior.AllowGet);
        }
    }
}