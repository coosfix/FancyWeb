using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{
    public class UsersListModel
    {
        public int? PhotoID { get; set; }
        public int UserID { get; set; }
        public bool Admin { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string OauthType { get; set; }
        public string RegisterDateTime { get; set; }
    }
}