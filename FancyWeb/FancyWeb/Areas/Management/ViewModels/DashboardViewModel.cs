using FancyWeb.Areas.Members.ViewModels;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{
    public class DashboardViewModel
    {
        public int DaytotalReven { get; set; }
        public int DayMembers { get; set; }
        public int DayOrders { get; set; }
        public int WaitShip { get; set; }
        public Dictionary<string, int> MemberSource { get; set; }
        public Dictionary<string, double[]> MemberGender { get; set; }
        public List<PopularProducts> PopularProducts { get; set; }
        public List<OrderHeader> recentOrders { get; set; }
        public List<EvaluationViewModel> recentEvaluation { get; set; }

        public DashboardViewModel()
        {
            this.PopularProducts = new List<PopularProducts>();
        }
    }
    public class PopularProducts
    {
        public int Pid { get; set; }
        public int count { get; set; }
        public string Pname { get; set; }
    }
    public class EvaluationViewModel
    {
        public int Puid { get; set; }
        public string productname { get; set; }
        public int Uid { get; set; }
        public string Username { get; set; }
        public string Comment { get; set; }
        public  string Date { get; set; }
        public string OrderNum { get; set; }
    }
}