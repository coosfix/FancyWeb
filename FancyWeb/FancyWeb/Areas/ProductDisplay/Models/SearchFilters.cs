using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.ProductDisplay.Models
{
    public class SearchFilters
    {
        public string Keyword { get; set; }
        public List<int> CategorySID { get; set; }
        public List<int> ColorID { get; set; }
        public List<int> SizeID { get; set; }
        public decimal MinSunitPrice { get; set; }
        public decimal MaxSunitPrice { get; set; }
        public int ActivityID { get; set; }
    }
}