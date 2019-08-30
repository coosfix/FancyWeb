using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{
    public class RegisterAdminModel
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }
        public int Region { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
    }
}