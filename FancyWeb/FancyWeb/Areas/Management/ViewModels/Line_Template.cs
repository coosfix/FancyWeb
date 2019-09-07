using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{
    public class Line_Template
    {

        public string type { get; set; }
        public string altText { get; set; }
        public Template template { get; set; }
        public Line_Template()
        {
            this.altText = "this is a carousel template";
            this.type = "template";
        }

    }
    public class Template
    {
        public string type { get; set; }
        public List<string> actions { get; set; }
        public List<Columns> columns { get; set; }
        public Template()
        {
            this.columns = new List<Columns>();
        }
    }
    public class Columns
    {
        public string thumbnailImageUrl { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public List<Action> actions { get; set; }

    }
    public class Action
    {
        public string type { get; set; }
        public string label { get; set; }
        public string uri { get; set; }
        public Action()
        {
            this.type = "uri";
        }
    }
}