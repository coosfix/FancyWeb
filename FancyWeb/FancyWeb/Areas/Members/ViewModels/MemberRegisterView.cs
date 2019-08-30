using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.ViewModels
{
    public class MemberRegisterView
    {
        public User newMember { get; set; }
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        public string UserName { get; set; }
        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string UserPassword { get; set; }
        [Required(ErrorMessage = "請輸入Email")]
        public string Email { get; set; }
        public int Region { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        [Required(ErrorMessage = "請輸入手機")]
        public string Phone { get; set; }

    }
}