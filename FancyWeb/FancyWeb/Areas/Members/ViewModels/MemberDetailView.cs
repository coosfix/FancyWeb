using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.ViewModels
{
    public class MemberDetailView
    {
        public string UserName { get; set; }
        public bool Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int City { get; set; }
        public int Region { get; set; }
        public string Address { get; set; }
    }
}