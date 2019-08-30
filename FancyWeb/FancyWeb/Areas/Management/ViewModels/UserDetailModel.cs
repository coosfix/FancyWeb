using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{
    public class UserDetailModel
    {
        public string UserName { get; set; }
        public bool Gender { get; set; }
        public bool Enabled { get; set; }
        public string RegisterDateTime { get; set; }
        public int totalcost { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public byte[] Password { get; set; }
        public string VerificationCode { get; set; }
        public string Address { get; set; }
    }
}