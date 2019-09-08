using FancyWeb.Areas.ProductDisplay.Models;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.ProductDisplay.Functions
{
    public class SearchMethod
    {
        public static IEnumerable<ProductCell> ClassifyResult(SearchFilters searchFilters)
        {
            FancyStoreEntities db = new FancyStoreEntities();

            IQueryable<ProductColor> preproducts = db.ProductColors.AsQueryable();

            var products = ProductMethod.CreateProductCells(preproducts).OrderBy(c => c.ProductID);

            IEnumerable<ProductCell> result;

            result = products.Where(p => p.ProductName.Contains(searchFilters.Keyword));

            if (result.Count() > 0)
            {
                if (searchFilters.CategorySID != null)
                {
                    result = result.Where(p => searchFilters.CategorySID.Contains(p.CategorySID));
                }
                if (searchFilters.ColorID != null)
                {
                    result = result.Where(p => searchFilters.ColorID.Contains(p.ColorID));
                }
                if (searchFilters.SizeID != null)
                {
                    switch (searchFilters.SizeID.Count)
                    {
                        case 1:
                            result = result.Where(p => p.SizeID.Contains(searchFilters.SizeID[0]));
                            break;
                        case 2:
                            result = result.Where(p => p.SizeID.Contains(searchFilters.SizeID[0]) || p.SizeID.Contains(searchFilters.SizeID[1]));
                            break;
                        case 3:
                            result = result.Where(p => p.SizeID.Contains(searchFilters.SizeID[0]) || p.SizeID.Contains(searchFilters.SizeID[1]) || p.SizeID.Contains(searchFilters.SizeID[2]));
                            break;
                        case 4:
                            result = result.Where(p => p.SizeID.Contains(searchFilters.SizeID[0]) || p.SizeID.Contains(searchFilters.SizeID[1]) || p.SizeID.Contains(searchFilters.SizeID[2]) || p.SizeID.Contains(p.SizeID[3]));
                            break;
                    }
                }
                if (searchFilters.MaxSunitPrice != 0)
                {
                    result = result.Where(p => p.SUnitPrice >= searchFilters.MinSunitPrice && p.SUnitPrice <= searchFilters.MaxSunitPrice);
                }
                if (searchFilters.ActivityID != 0)
                {
                    var activityname = db.Activities.Where(a => a.ActivityID == searchFilters.ActivityID).FirstOrDefault().ActivityName;
                    result = result.Where(p => p.ActivityName == activityname);
                }
            }
            return result;
        }
    }
}