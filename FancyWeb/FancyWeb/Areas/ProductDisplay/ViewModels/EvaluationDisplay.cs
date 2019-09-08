using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.ProductDisplay.ViewModels
{
    public class EvaluationDisplay
    {
        public string UserPhoto { get; set; }
        public string OrderNum { get; set; }
        public string Comment { get; set; }
        public string ProductPhoto { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string EvaluationDate { get; set; }
        public int Grade { get; set; }
        public int Other { get; set; }
    }
}