using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.ProductDisplay.Models
{
    public class CategoryFilter
    {
        public string FilterName { get; set; }
        public Dictionary<string, int> Filter { get; set; }
    }
}