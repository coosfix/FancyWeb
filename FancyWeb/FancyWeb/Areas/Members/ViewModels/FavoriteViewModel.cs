using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.ViewModels
{
    public class FavoriteViewModel
    {
        public List<FavoriteView> Favorlist { get; set; }

        public Dictionary<string, int> CompanyCount()
        {
            var tt = this.Favorlist.GroupBy(n => n.CompanyName).Select(n => new
            {
                n.Key,
                companyn = n.Count()
            });
            Dictionary<string, int> keyValues = new Dictionary<string, int>();
            foreach (var item in tt)
            {
                keyValues.Add(item.Key, item.companyn);
            }
            return keyValues;
        }

        public Dictionary<string, int> ActivityCount()
        {
            var tt = this.Favorlist.GroupBy(n => n.Activity).Select(n => new
            {
                n.Key,
                activity = n.Count()
            });
            Dictionary<string, int> keyValues = new Dictionary<string, int>();
            foreach (var item in tt)
            {
                if (item.Key != null)
                {
                    keyValues.Add(item.Key, item.activity);
                }
            }
            return keyValues;
        }
    }
}