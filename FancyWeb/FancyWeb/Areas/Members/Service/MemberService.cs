using FancyWeb.Areas.Members.ViewModels;
using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Members.Service
{
    public class MemberService
    {
        private FancyStoreEntities db = new FancyStoreEntities();
        public static string upid { get; set; }
        #region 登入確認
        public bool LoginCheck(string UserName, string Password)
        {
            if (db.Users.Any(m => m.UserName == UserName))
            {
                User LoginUser = db.Users.Where(m => m.UserName == UserName).FirstOrDefault();
                byte[] pw = MemberMethod.HashPw(Password, LoginUser.GUID);
                upid = LoginUser.UserID.ToString();
                return BitConverter.ToString(pw) == BitConverter.ToString(LoginUser.UserPassword)
                    && LoginUser.VerificationCode==String.Empty;
            }
            else { return false; }
        }
        #endregion

        #region 註冊新會員
        public bool Register(User RegisterMember)
        {
            try
            {
                db.Users.Add(RegisterMember);
                db.SaveChanges();
                upid = RegisterMember.UserID.ToString();
                return true;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }
        }
        #endregion
        #region 是否為Admin
        public bool isAdmin(string id)
        {
            return db.Users.Find(Convert.ToInt32(id)).Admin;
        }
        #endregion
        #region 更新密碼
        public string UpdatePassword(string NewPw, string Account)
        {
            try
            {
                var data = db.Users.FirstOrDefault(n => n.UserName == Account);
                string guid = Guid.NewGuid().ToString("N");
                string newverification = MemberMethod.GetNewPW();
                byte[] hashPw = MemberMethod.HashPw(NewPw, guid);
                data.UserPassword = hashPw;
                data.GUID = guid;
                data.VerificationCode = newverification;
                db.SaveChanges();
                return newverification;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 帳號檢查是否重複
        public bool AccountCheck(string Account)
        {
            return db.Users.Any(n => n.UserName == Account);
        }
        #endregion

        #region 檢查信箱是否重複
        public bool EmailCheck(string Email)
        {
            return db.Users.Any(n => n.Email == Email);
        }
        #endregion

        #region 使用者基本資料
        public MemberDetailView UserDetail(int id)
        {
            User user = db.Users.Find(id);
            MemberDetailView vdata = new MemberDetailView()
            {
                UserName = user.UserName,
                Gender = user.Gender,
                Email = user.Email,
                Phone = user.Phone,
                City = user.Region.CityID,
                Region = user.RegionID,
                Address = user.Address,
            };

            return vdata;
        }
        #endregion

        #region 修改使用者基本資料
        public bool ChangeUserDetail(MemberDetailView data, int id)
        {
            try
            {
                User user = db.Users.Find(id);
                user.Email = data.Email;
                user.Phone = data.Phone;
                user.RegionID = data.Region;
                user.Address = data.Address;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region 修改密媽
        public bool ChangePW(string Email, string OldPW, string NewPW)
        {
            try
            {
                int id = Int32.Parse(upid);
                string oldguid = db.Users.Find(id).GUID;
                byte[] oldpp = MemberMethod.HashPw(OldPW, oldguid);
                if (db.Users.Any(n => n.Email == Email && n.UserPassword == oldpp))
                {
                    User user = db.Users.Find(id);
                    string NewGUID = Guid.NewGuid().ToString("N");
                    user.UserPassword = MemberMethod.HashPw(NewPW, NewGUID);
                    user.GUID = NewGUID;
                    db.SaveChanges();
                    return true;
                }
                else { return false; }

            }
            catch (Exception ex)
            {
                string me = ex.Message;
                return false;
            }
        }
        #endregion
        #region 清空驗證
        public  bool ClearV(string PV)
        {
            var data = db.Users.Where(n => n.VerificationCode == PV).FirstOrDefault();
            if (data == null) return true;
            data.VerificationCode = String.Empty;
            db.SaveChanges();
            return true;
        }
        #endregion
        #region 取得使用者圖片
        public byte[] GetUserImg(int? id)
        {
            return db.Users.Find(id).Photo.Photo1;
        }
        #endregion

        #region 上傳使用者頭像
        public bool UploadUIMG(HttpPostedFileBase data, int id)
        {
            if (data != null)
            {
                //將圖轉成二進位
                var imgSize = data.ContentLength;
                byte[] imgByte = new byte[imgSize];
                data.InputStream.Read(imgByte, 0, imgSize);
                int? phid = db.Users.Find(id).PhotoID;
                Photo photo = db.Photos.Find(phid);
                photo.Photo1 = imgByte;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 取得使用者收藏
        public List<FavoriteView> GetFavoritelist(int? id)
        {
            List<FavoriteView> favorites = new List<FavoriteView>();
            var myFavorite = db.Users.Find(id).MyFavorites;
            foreach (var item in myFavorite)
            {
                favorites.Add(new FavoriteView
                {
                    FavoriteID = item.FavoriteID,
                    ProductID = item.ProductID,
                    ProductName = item.Product.ProductName,
                    CompanyName = item.Product.Supplier.SupplierName + "-" + item.Product.SupplierID,
                    Activity = item.Product.ActivityProducts.Any(n => n.ProductID == item.ProductID) ?
                    item.Product.ActivityProducts.Where(n => n.ProductID == item.ProductID).FirstOrDefault().Activity.ActivityName + "-" + item.Product.ActivityProducts.Where(n => n.ProductID == item.ProductID).FirstOrDefault().Activity.ActivityID : null,
                    UnitePrice = item.Product.UnitPrice,
                    Discount = item.Product.ActivityProducts.Any(n => n.ProductID == item.ProductID) ? item.Product.ActivityProducts.Where(n => n.ProductID == item.ProductID).FirstOrDefault().Activity.DiscountMethod.Discount : decimal.One,
                    CreateDate = item.CreateDate
                });
            }
            return favorites;
        }
        #endregion
        #region 收藏移除
        public bool FavoriteRemove(int? id)
        {
            try
            {
                var f = db.MyFavorites.Find(id);
                db.MyFavorites.Remove(f);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region 收藏過濾
        public List<FavoriteView> FavoritesFilter(FilterClass filter)

        {
            List<FavoriteView> filterlist;
            switch (filter.orderby)
            {
                case "asc":
                    filterlist = filter.list.OrderBy(n => n.UnitePrice * n.Discount).ToList();
                    break;
                case "desc":
                    filterlist = filter.list.OrderByDescending(n => n.UnitePrice * n.Discount).ToList();
                    break;
                default:
                    filterlist = filter.list.OrderByDescending(n => n.CreateDate).ToList();
                    break;
            }

            if (filter.companyfilter != null)
            {
                filterlist = filterlist.Where(n => filter.companyfilter.Contains(n.CompanyName.Split('-')[1])).ToList();
            }
            if (filter.activtyfilter != null)
            {
                filterlist = filterlist.Where(n => filter.activtyfilter.Contains((n.Activity ?? "X-D").Split('-')[1])).ToList();
            }
            if (filter.Keyword != null)
            {
                filterlist = filterlist.Where(n => n.ProductName.Contains(filter.Keyword)).ToList();
            }
            return filterlist;
        }
        #endregion

        #region 使用者訂單
        public List<OrderHeaderView> GetOrderHeader(int? id)
        {
            List<OrderHeaderView> orderslist = new List<OrderHeaderView>();
            var myOrder = db.Users.Find(id).OrderHeaders;
            foreach (var item in myOrder)
            {
                orderslist.Add(new OrderHeaderView
                {
                    OrderID = item.OrderID,
                    OrderNum = item.OrderNum,
                    OrderStatus = item.OrderStatusList.OrderStatusName,
                    OrderDate = item.OrderDate,
                    ShipDate = item.ShipDate ?? null,
                    Shipping = item.Shipping.ShippingName,
                    OrderAmount = item.OrderAmount,
                    PayMethod = item.PayMethod.PayMethodName,
                    Address = $"{ item.User.Region.City.CityName} {item.User.Region.RegionName} {item.Address}",
                    Phone = item.Phone
                });
            }
            return orderslist;
        }
        #endregion

        #region 訂單過濾
        public List<OrderHeaderView> OrderHeaderFilter(OrderFilter filter)
        {
            List<OrderHeaderView> filterlist;
            switch (filter.orderby)
            {
                case "asc":
                    filterlist = filter.list.OrderBy(n => n.OrderAmount).ToList();
                    break;
                case "desc":
                    filterlist = filter.list.OrderByDescending(n => n.OrderAmount).ToList();
                    break;
                default:
                    filterlist = filter.list.OrderByDescending(n => n.OrderDate).ToList();
                    break;
            }

            if (filter.Statusfilter != null)
            {
                filterlist = filterlist.Where(n => filter.Statusfilter.Contains(n.OrderStatus)).ToList();
            }

            if (filter.OrderDate != null)
            {
                filterlist = filterlist.Where(n => filter.OrderDate.Contains(n.OrderDate.Year+"/"+n.OrderDate.Month)).ToList();
            }
            return filterlist;
        }
        #endregion
        #region 訂單明細
        public List<OrderDetailView> OrderDetail(int oid)
        {
            var od = db.OrderDetails.Where(n => n.OrderID == oid);
            List<OrderDetailView> orderDetails = new List<OrderDetailView>();
            foreach (var item in od)
            {
                orderDetails.Add(new OrderDetailView
                {
                    ProductName = item.Product.ProductName,
                    Color = item.ProductColor.Color.ColorName,
                    Size = item.ProductSize.Size.SizeName,
                    QTY = item.OrderQTY,
                    UnitPrice = item.UnitPrice,
                    subtotal = item.OrderQTY* item.UnitPrice,
                    Freight = item.OrderHeader.PayMethod.Freight,
                });
            }
            return orderDetails;
        }
        #endregion
        #region 城市資料
        public IEnumerable<CityJson> CityJson()
        {
            return db.Cities.Select(n => new CityJson { id = n.CityID, name = n.CityName }).ToList();
        }
        #endregion

        #region 地區資料
        public IEnumerable<ReginJson> RegionJson(int? cid = 3)
        {
            return db.Regions.Where(n => n.CityID == cid).Select(n => new ReginJson { id = n.RegionID, name = n.RegionName }).ToList();
        }
        #endregion
    }

    public class ReginJson
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class CityJson
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}