using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace FancyWeb.Areas.Management.ViewModels
{

    public class ReplyBody<T>
    {
        public string replyToken { get; set; }
        public List<T> messages { get; set; }
    }
    public class LINE_Reply<T>
    {
        public const string API_URL = "https://api.line.me/v2/bot/message/reply";
        private WebRequest req;
        public string access_token = "hzY1eF/bT9gpLEz1jCBStWba58mwojDO5NCKlbc6oM29PpIRUhtZkl6y05Ny9VvB7yTVQm9nx+lSWANdIt8qfegx7nypPjBroJ0HXuX0EAAmqMK0dl87H0++9D3G+NpObaDVSuAnzKWAH68iLF+KUQdB04t89/1O/w1cDnyilFU=";
        public LINE_Reply(ReplyBody<T> body)
        {
            //--- set header and body required infos ---
            req = WebRequest.Create(API_URL);
            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers["Authorization"] = "Bearer " + access_token;
            // --- format to json and add to request body ---
            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                string datas = JsonConvert.SerializeObject(body);
                streamWriter.Write(datas);
                streamWriter.Flush();
            }
        }
        /*
            --- send message to LINE ---
            return response data
        */
        public string send()
        {
            string result = null;
            try
            {
                WebResponse response = req.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return result;
        }
    }
}