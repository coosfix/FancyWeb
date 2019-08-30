using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using FancyWeb.Models;
using static FancyWeb.Areas.Backend.Models.users;

namespace FancyWeb.Areas.Backend.Models
{
    public class CharHub : Hub
    {
        FancyStoreEntities db = new FancyStoreEntities();

        //搜尋會員(左上方) Note-10
        public void searchuser(int id, string str)
        {
            //讀出除了自己之外的全部user , 全部人員列表(下方) Note-03
            var q = db.Users.Where(x => x.UserID != id && x.UserName.Contains(str));

            foreach (var x in q)
            {
                Clients.Client(Context.ConnectionId).userlist(x.UserID, x.UserName, x.Email, (int)x.PhotoID, x.Phone);
            }
        }


        //登入作業 Note-01
        public void login(int id, string name)
        {
            //重新登入時, 將原本已登入的user做登出動作
            //if (onlineuser.ContainsKey(id))
            //{
            //    //登出作業  Note-02  
            //    Clients.Client(onlineuser[id].connectid).logout();
            //    onlineuser.Remove(id);
            //}

            connectuser newuser = new connectuser();

            newuser.connectid = Context.ConnectionId;
            newuser.name = name;
            onlineuser.Add(id, newuser);

            //讀出除了自己之外的全部user , 全部人員列表(左上方) Note-03
            var q = db.Users.Where(x => x.UserID != id);
            foreach (var x in q)
            {
                Clients.Client(Context.ConnectionId).userlist(x.UserID, x.UserName, x.Email, (int)x.PhotoID, x.Phone);
            }

            //有對話過的使用者
            List<forlogin> loginlist = new List<forlogin>();

            //讀出login使用者的全部有過對話的使用者訊息 (左下方) Note-04
            var messagelog = db.Messages.Where(x => (x.FromUserID == id) || (x.ToUserID == id && x.FromUserID != x.ToUserID)).GroupBy(x => x.FromUserID == id ? x.ToUserID : x.FromUserID);

            Dictionary<int, string> intlist = new Dictionary<int, string>();
            Dictionary<int, DateTime> intdate = new Dictionary<int, DateTime>();

            //存放每位上線的使用者的最後一筆訊息(不包含自己)
            foreach (var x in messagelog.Where(a => a.Key != id))
            {
                intlist.Add(x.Key, x.Last().TalkContent);
                intdate.Add(x.Key, x.Last().CreateDate);
            }

            foreach (var a in intlist)
            {
                var user = db.Users.Where(x => x.UserID == a.Key).Select(x => new { x.UserID, x.UserName, x.PhotoID }).FirstOrDefault();

                var isreadCnt = db.Messages.Where(x => x.FromUserID == a.Key && x.ToUserID == id && x.IsRead == false).Count();

                ////客服進來只會顯示未讀的user, 一般會員則會顯示service
                //if (name == "service" && isreadCnt > 0)
                //{
                //    continue;
                //}

                forlogin _forlogin = new forlogin();

                try
                {
                    string talkdetail = a.Value.Substring(0, 10);

                    _forlogin.id = a.Key;
                    _forlogin.name = user.UserName;
                    _forlogin.talkcontent = talkdetail;
                    _forlogin.isreadCnt = isreadCnt;
                    _forlogin.datetime = intdate[a.Key];
                    _forlogin.photoid = (int)user.PhotoID;
                }
                catch
                {
                    _forlogin.id = a.Key;
                    _forlogin.name = user.UserName;
                    _forlogin.talkcontent = a.Value;
                    _forlogin.isreadCnt = isreadCnt;
                    _forlogin.datetime = intdate[a.Key];
                    _forlogin.photoid = (int)user.PhotoID;

                }
                finally
                {
                    loginlist.Add(_forlogin);
                }
            }

            //聊天人員列表 (左下方)   Note-04
            foreach (var a in loginlist.OrderByDescending(x => x.isreadCnt).ThenByDescending(x => x.datetime))
            {
                Clients.Client(Context.ConnectionId)._messageuser(a.id, a.name, a.talkcontent, a.isreadCnt, a.photoid);
            }


            //上線時將使用者由"離線"改為"上線",並排到最上層  Note-01
            foreach (var a in onlineuser)
            {
                Clients.Client(Context.ConnectionId)._login(a.Key);
            }

            Clients.All._login(id);
        }

        //離線 Note-02
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            // 當使用者離開時，移除在清單內的 ConnectionId
            // 呼叫client端   Clients.All.removeList(Context.ConnectionId);
            var id = onlineuser.Where(a => a.Value.connectid == Context.ConnectionId).ToList();
            if (id.Count > 0)
            {
                Clients.All._logout(id.First().Key);
                onlineuser.Remove(id.First().Key);
            }
            return base.OnDisconnected(stopCalled);
        }

        //已讀  Note-05
        public void isread(int fromid, int toid)
        {
            var q = db.Messages.Where(x => x.FromUserID == toid && x.ToUserID == fromid && x.IsRead == false);

            foreach (var a in q)
            {
                a.IsRead = true;
                Clients.Client(onlineuser[toid].connectid).isread(fromid, toid, a.MessageID);
            }
            db.SaveChanges();
        }

        //發送訊息 Note-06
        public void sendMsg(int fromid, int toid, string message)
        {
            Message msg = new Message();
            msg.FromUserID = fromid;
            msg.ToUserID = toid;
            msg.CreateDate = DateTime.Now;
            msg.TalkContent = message;
            msg.IsRead = false;
            db.Messages.Add(msg);
            db.SaveChanges();

            var msgid = db.Messages.Select(x => x.MessageID).ToList().Last();
            var photoid = db.Users.Find(fromid).PhotoID;

            //在呼叫端顯示
            Clients.Client(Context.ConnectionId).sendMessage(fromid, onlineuser[fromid].name, toid, message, msg.CreateDate.ToString("MM/dd H:mm"), msgid, photoid);

            //被叫端若有上線則顯示訊息
            if (onlineuser.ContainsKey(toid) && fromid != toid)
            {
                Clients.Client(onlineuser[toid].connectid).sendMessage(fromid, onlineuser[fromid].name, toid, message, msg.CreateDate.ToString("MM/dd H:mm"), msgid, photoid);
            }
        }

        //點選欲回應的user  Note-07
        public void callmessage(int fromid, int toid)
        {
            if (toid > 0)
            {
                var message = db.Messages.Where(x => (x.FromUserID == fromid && x.ToUserID == toid) || (x.FromUserID == toid && x.ToUserID == fromid)).Select(x => new
                {
                    x.MessageID,
                    x.FromUserID,
                    FromUserName = x.User.UserName,
                    x.ToUserID,
                    x.TalkContent,
                    x.CreateDate,
                    x.User.PhotoID,
                    x.IsRead
                });

                foreach (var a in message)
                {
                    string date = a.CreateDate.ToString("MM/dd H:mm ");

                    //訊息內容   Note-08
                    Clients.Client(Context.ConnectionId).messagelog(a.FromUserID, a.FromUserName, a.TalkContent, date, a.MessageID, a.PhotoID, a.IsRead);
                }
            }
        }




    }

    //定義連線使用者
    public static class users
    {
        public static Dictionary<int, connectuser> onlineuser = new Dictionary<int, connectuser>();

        public class connectuser
        {
            public string connectid;
            public string name;
        }
    }

    //有對話訊息的user(上方)
    public class forlogin
    {
        public int id;
        public string name;
        public string talkcontent;
        public int isreadCnt;
        public DateTime datetime;
        public int photoid;

    }
}