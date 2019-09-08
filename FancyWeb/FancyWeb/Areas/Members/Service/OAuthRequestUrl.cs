using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.Service
{
    public class OAuthRequestUrl
    {
        static string redirect_uri = HttpUtility.UrlEncode("https://localhost:44395/Login/callback");
        //static string redirect_uri = HttpUtility.UrlEncode("https://192.168.39.25:7000/Login/callback");
        static public string LineUrl()
        {
            string client_id = "1595990013";
            string response_type = "code";
            string Provider = "LINE";
            string state = "666-" + Provider;
            string LineLoginUrl = $@"https://access.line.me/oauth2/v2.1/authorize?response_type={response_type}&client_id={client_id}&redirect_uri={redirect_uri}&state={state}&scope=openid%20profile%20email&nonce=09876xyz";

            return LineLoginUrl;
        }
        static public string FBeUrl()
        {
            string client_id = "459426044646311";
            string Provider = "Facebook";
            string state = "666-" + Provider;
            string FBLoginUrl = $@"https://www.facebook.com/v3.3/dialog/oauth?&client_id={client_id}&redirect_uri={redirect_uri}&state={state}&scope=public_profile%2C%20email";

            return FBLoginUrl;
        }
        static public string GoogleeUrl()
        {
            string client_id = "549733988342-10gk6flc0t8dn0349c7i4mrgj3mda5n2.apps.googleusercontent.com";
            string Provider = "Google";
            string state = "666-" + Provider;
            string GoogleLoginUrl = $@"https://accounts.google.com/o/oauth2/v2/auth?state={state}&client_id={client_id}&response_type=code&redirect_uri={redirect_uri}&scope=email%20profile+https://www.googleapis.com/auth/userinfo.profile";

            return GoogleLoginUrl;
        }
    }
}