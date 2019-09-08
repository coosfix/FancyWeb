using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FancyWeb.Areas.CheckProcess.Function
{
    public class CheckMethod
    {
        //寄送Email
        /// <summary>
        /// 寄送目標Email , 使用者Username , 新密碼 NewPassword
        /// </summary>
        public static void SendEmail(string Email,string tempmail)
        {
            NetworkCredential login;
            MailMessage msg = new MailMessage();
            login = new NetworkCredential("fancydayevery", "msit12201");//登入寄件者
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.EnableSsl = true;
            SmtpServer.Credentials = login;
            msg.From = new MailAddress("fancydayevery@gmail.com", "Fancy");//寄件者
            msg.To.Add(Email);//可多個
            msg.Subject = "訂單成立確認信"; //設定信件主旨
            msg.IsBodyHtml = true;//設定信件內容為HTML格式 
            //string tempmail = System.IO.File.ReadAllText("../Email/mail.html");//讀取html
            msg.Body = tempmail; //設定信件內容 
            SmtpServer.Send(msg);//送出
        }

        //製作認證用Email內容
        /// <summary>
        /// Email樣式 認證用 html檔 , ...., ....
        /// </summary>
        /// <param name="TempString"></param>
        /// <param name="UserName"></param>
        /// <param name="NewPW"></param>
        /// <returns></returns>
        public static string VerificationCodeMailBody(string TempString,OrderHeader orderHeader, string ordermail)
        {
            //將使用者資料填入
            TempString = TempString.Replace("{{訂單編號}}", orderHeader.OrderNum);
            TempString = TempString.Replace("{{訂購者姓名}}", orderHeader.ReceipterName);
            TempString = TempString.Replace("{{訂單連結}}", ordermail);
            //TempString = TempString.Replace("{{訂購者日期}}", orderHeader.OrderDate.ToString());
            //回傳最後結果
            return TempString;
        }
    }
}