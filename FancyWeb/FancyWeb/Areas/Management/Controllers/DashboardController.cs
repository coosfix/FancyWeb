using FancyWeb.Areas.Management.Service;
using FancyWeb.Areas.Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FancyWeb.Areas.Management.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Management/Dashboard
        public ActionResult Index()
        {
            DashboardService service = new DashboardService();
            ViewBag.yearlist = service.YearJson();
            return View(service.GetallData());
        }
        //類別營收占比Json
        [HttpGet]
        public ActionResult Totalpercent()
        {
            DashboardService service = new DashboardService();
            return Json(service.Totalpercent(), JsonRequestBehavior.AllowGet);
        }

        //近三年類別銷售成長趨勢
        [HttpGet]
        public ActionResult YearTop3growing()
        {
            DashboardService service = new DashboardService();

            return Json(service.YearTop3growing(), JsonRequestBehavior.AllowGet);
        }

        //地區銷售
        [HttpGet]
        public ActionResult RegionSell()
        {
            DashboardService service = new DashboardService();

            return Json(service.RegionSell(), JsonRequestBehavior.AllowGet);
        }

        //總營收
        [HttpGet]
        public ActionResult Totalrevenue()
        {
            DashboardService service = new DashboardService();

            return Json(service.Totalrevenue(), JsonRequestBehavior.AllowGet);
        }
        //全部下單訂單日期
        [HttpGet]
        public ActionResult AllOrderDate(int? year)
        {
            DashboardService service = new DashboardService();
            return Json(service.AllOrderDate(year), JsonRequestBehavior.AllowGet);
        }

        LineBotService linemesssage = new LineBotService();
        [HttpPost]
        public ActionResult LineBot(string destination, LINEModel data)
        {
            if (data == null || data.events == null) return Json("BadRequest");
            string replyToken = data.events[0].replyToken;
            if (data.events[0].message.text == "推薦")
            {
                UriBuilder uriBuilder = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("ByteImage", "Product", new { area = "ProductDisplay" })
                };
                string url = uriBuilder.ToString();
                ReplyBody<Line_Template> _Reply = new ReplyBody<Line_Template>()
                {
                    replyToken = replyToken,
                    messages = linemesssage.Line_Templates(url)
                };
                LINE_Reply<Line_Template> reply = new LINE_Reply<Line_Template>(_Reply);
                reply.send();
            }
            else
            {
                ReplyBody<SendMessage> _Reply = new ReplyBody<SendMessage>()
                {
                    replyToken = replyToken,
                    messages = procMessage(data.events[0].message)
                };
                LINE_Reply<SendMessage> reply = new LINE_Reply<SendMessage>(_Reply);
                reply.send();
            }

            return Json("Hi", JsonRequestBehavior.AllowGet);
        }

        private List<SendMessage> procMessage(ReceiveMessage m)
        {
            List<SendMessage> msgs = new List<SendMessage>();
            UriBuilder uriBuilder = new UriBuilder(Request.Url)
            {
                Path = Url.Action("GetProductDetail", "Product", new { area = "ProductDisplay" })
            };
            string url = uriBuilder.ToString();
            SendMessage sm = new SendMessage()
            {
                type = Enum.GetName(typeof(MessageType), m.type)
            };
            switch (m.type)
            {
                case MessageType.sticker:
                    sm.packageId = m.packageId;
                    sm.stickerId = m.stickerId;
                    break;
                case MessageType.text:
                    if (m.text == "今日熱門")
                    {
                        sm.text = linemesssage.Getpupp(url);
                    }
                    else if (m.text == "活動商品")
                    {
                        sm.text = linemesssage.GetActityP(url);
                    }
                    else if (m.text.Split(' ')[0] == "!訂單取消")
                    {
                        string[] vs = m.text.Split(' ');
                        sm.text = linemesssage.CancelOrder(vs[1], vs[2]);
                    }
                    else if (m.text.Split(' ')[0] == "!訂單查詢")
                    {

                        string[] vs = m.text.Split(' ');
                        var data = linemesssage.SearchOrder(vs[1], vs[2]);
                        foreach (var item in data)
                        {
                            SendMessage smm = new SendMessage()
                            {
                                type = Enum.GetName(typeof(MessageType), m.type)
                            };
                            smm.text = $"📜訂單編號:{item.ordernum}\n訂單狀態:{item.orderstatus}\n訂單總額:NT${item.amount}\n" +
                                $"=============";
                            foreach (var item2 in item.orderdetail)
                            {
                                smm.text += $"\n📋商品名稱:{item2.pname}\n購買數量:{item2.pQTY}\n價格:NT$ {item2.pUP}\n------------";
                            }
                            msgs.Add(smm);
                        }
                        return msgs;
                    }
                    break;
                default:
                    sm.type = Enum.GetName(typeof(MessageType), MessageType.text);
                    sm.text = "很抱歉，我只是一隻回音機器人，目前只能回覆基本貼圖與文字訊息喔！";
                    break;
            }
            msgs.Add(sm);
            return msgs;
        }
    }
}