using FancyWeb.Areas.Management.ViewModels;
using FancyWeb.Areas.Members.Service;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Management.Service
{
    public class AdminService
    {
        private FancyStoreEntities db = new FancyStoreEntities();


        public List<UsersListModel> GetUserList()
        {
            var Userlist = db.Users.ToList();
            List<UsersListModel> AllUser = new List<UsersListModel>();
            foreach (var item in Userlist)
            {
                AllUser.Add(new UsersListModel
                {
                    PhotoID = item.PhotoID,
                    UserID = item.UserID,
                    UserName = item.UserName,
                    Phone = item.Phone,
                    Email = item.Email,
                    Admin = item.Admin,
                    OauthType = item.OauthType,
                    Enabled = item.Enabled,
                    RegisterDateTime = item.RegistrationDate.ToString("yyyy/MM/dd")
                });
            }

            return AllUser;
        }
        #region 使用者詳細資料
        public UserDetailModel GetUserDetial(int id)
        {
            var udata = db.Users.Find(id);
            UserDetailModel model = new UserDetailModel
            {
                UserName = udata.UserName,
                Password = udata.UserPassword,
                Address = $"{udata.Region.City.CityName} {udata.Region.RegionName} {udata.Address}",
                Email = udata.Email,
                Phone = udata.Phone,
                VerificationCode = udata.VerificationCode,
                Enabled = udata.Enabled,
                Gender = udata.Gender,
                totalcost = udata.OrderHeaders.Sum(n => n.OrderAmount),
                RegisterDateTime = udata.RegistrationDate.ToString("yyyy/MM/dd")
            };
            return model;
        }
        #endregion
        #region 修改使用者資料
        public bool AdminUserControl(int id,string NewPW,string guid, string tempmail ,string ValidateUrl)
        {
            try
            {
                var user = db.Users.Find(id);
                user.UserPassword = MemberMethod.HashPw(NewPW, guid);
                user.GUID = guid;
                db.SaveChanges();
                MemberMethod.SendEmail(user.Email, user.UserName, NewPW, MemberMethod.VerificationCodeMailBody(tempmail, user.UserName,
                NewPW, ValidateUrl.Replace("%3F", "?")));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
        #region 黑名單
        public  bool DoBan(int id)
        {
            try
            {
                var user = db.Users.Find(id);
                user.Enabled = !user.Enabled;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
        #region 重新寄送驗證URl
        //public bool AdminUserControl(int id, string tempmail,string VC)
        //{
        //    try
        //    {
        //        var user = db.Users.Find(id);
        //        user.VerificationCode = VC;
        //        db.SaveChanges();
        //        Members.Service.MemberMethod.SendEmail(user.Email, user.UserName, VC, tempmail);
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        #endregion
        #region 寄送信息
        public bool SendEmail(string tempmail,string Email ,string UserName ,string content)
        {
            try
            {
                Members.Service.MemberMethod.SendEmail(Email, UserName, content, tempmail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region 註冊新會員
        public int Register(User RegisterMember)
        {
            try
            {
                db.Users.Add(RegisterMember);
                db.SaveChanges();
                
                return RegisterMember.UserID;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return 0;
            }
        }
        #endregion
        #region 取得使用者圖片
        public byte[] GetUserImg(int id)
        {
            return db.Photos.Find(id).Photo1;
        }
        #endregion
        #region 取得base64
        public string Getbase64(int id, int? colorid = 0)
        {
            if (colorid == 0)
            {
                colorid = db.ProductColors.Where(c => c.ProductID == id).Select(c => c.ColorID).First();
            }
            int? pid = db.ProductColors.Where(p => p.ProductID == id && p.ColorID == colorid).Select(p => p.PhotoID).First();
            byte[] by = db.Photos.Find(pid).Photo1;
            var img = Convert.ToBase64String(by);
            return img;
        }
        #endregion

    }
}