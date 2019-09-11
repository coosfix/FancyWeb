using Facebook;
using JWT.Builder;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace FancyWeb.Areas.Members.Service
{
    public class OAuthMethod
    {
        //static string redirect_uri = "https://localhost:44395/Login/callback";
        //static string redirect_uri = "https://192.168.39.25:7000/Login/callback";
        static string redirect_uri = "https://msit12201.azurewebsites.net/Login/callback";
        static public Dictionary<string, string> LineResponse(string code, string state)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            string client_secret = "7a8a676c31cef21becc40e57d21a575c";
            if (state.Contains("666"))
            {
                #region Api變數宣告
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                NameValueCollection nvc = new NameValueCollection();
                #endregion
                try
                {
                    string ApiUrl_Token = "https://api.line.me/oauth2/v2.1/token";
                    nvc.Add("grant_type", "authorization_code");
                    nvc.Add("code", code);
                    nvc.Add("redirect_uri", redirect_uri);
                    nvc.Add("client_id", "1595990013");
                    nvc.Add("client_secret", client_secret);
                    string resultJson = Encoding.UTF8.GetString(wc.UploadValues(ApiUrl_Token, "POST", nvc));
                    dynamic data = JObject.Parse(resultJson);
                    string token = data.id_token;
                    dynamic payload = new JwtBuilder().WithSecret(client_secret).MustVerifySignature().Decode<Dictionary<string, object>>(token);
                    keyValues.Add("ID", payload["sub"]);
                    keyValues.Add("email", payload["email"]);
                    keyValues.Add("name", payload["name"]);
                    keyValues.Add("picture", payload["picture"]);
                    return keyValues;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    keyValues.Add("Message", msg);
                    return keyValues;
                }
            }
            else return null;
        }

        static public Dictionary<string, string> FBResponse(string code, string state)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            string client_secret = "b8b48eeba9e6dc976a471ed9c3de1a18";
            if (state.Contains("666"))
            {
                #region Api變數宣告
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                NameValueCollection nvc = new NameValueCollection();
                #endregion
                try
                {
                    string ApiUrl_Token = "https://graph.facebook.com/v3.3/oauth/access_token?";
                    nvc.Add("code", code);
                    nvc.Add("redirect_uri", redirect_uri);
                    nvc.Add("client_id", "459426044646311");
                    nvc.Add("client_secret", client_secret);
                    string resultJson = Encoding.UTF8.GetString(wc.UploadValues(ApiUrl_Token, "POST", nvc));
                    dynamic data = JObject.Parse(resultJson);
                    string token = data.access_token;
                    FacebookClient fb = new FacebookClient(token);
                    dynamic payload = fb.Get("https://graph.facebook.com/me?", new { fields = "name,email,picture" });
                    keyValues.Add("ID", payload.id);
                    keyValues.Add("email", payload.email);
                    keyValues.Add("name", payload.name);
                    string pictureUrl = $"https://graph.facebook.com/{payload.id}/picture?type=large";
                    keyValues.Add("picture", pictureUrl);
                    return keyValues;
                }
                catch (Exception ex)
                {
                    keyValues.Add("Message", ex.Message);
                    return keyValues;
                }
            }
            else
                return null;
        }

        static public Dictionary<string, string> GoogleResponse(string code, string state)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            if (state.Contains("666"))
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    NameValueCollection nvc = new NameValueCollection();
                    try
                    {
                        string client_id = "549733988342-10gk6flc0t8dn0349c7i4mrgj3mda5n2.apps.googleusercontent.com";
                        string client_secret = "N0oy8BXPtTk26j5GBxGgOtqS";
                        string url = $"https://www.googleapis.com/oauth2/v4/token?client_id={client_id}&client_secret={client_secret}&redirect_uri={redirect_uri}&grant_type=authorization_code&code={code}&access_type=offline";
                        string resultJson = Encoding.UTF8.GetString(wc.UploadValues(url, "POST", nvc));
                        dynamic data = JObject.Parse(resultJson);
                        string access_token = data.access_token;
                        string dataUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + access_token;
                        dynamic payload = JObject.Parse(wc.DownloadString(dataUrl));
                        keyValues.Add("ID", payload.id.ToString());
                        keyValues.Add("email", payload.email.ToString());
                        keyValues.Add("name", payload.name.ToString());
                        keyValues.Add("picture", payload.picture.ToString());
                        return keyValues;
                    }
                    catch (WebException ex)
                    {
                        keyValues.Add("Message", ex.Message);
                        return keyValues;
                    }
                }
            }
            else
                return null;
        }
    }
}