using FancyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.ProductDisplay.Models
{
    public class ProductMethod
    {
        public static List<CartItem> Cart(int id)
        {
            using (FancyStoreEntities db = new FancyStoreEntities())
            {
                //回傳要顯示的購物車資料
                List<CartItem> cart;
                var usercart = db.Carts.Where(c => c.UserID == id).ToList();
                cart = new List<CartItem>();
                CartItem cartItem;

                foreach (var uitem in usercart)
                {
                    cartItem = new CartItem()
                    {
                        ProductID = uitem.ProductID,
                        ProductName = db.Products.Find(uitem.ProductID).ProductName,
                        ProductColorID = uitem.ProductColorID,
                        ColorID = db.ProductColors.Find(uitem.ProductColorID).Color.ColorID,
                        ColorName = db.ProductColors.Find(uitem.ProductColorID).Color.ColorName,
                        ProductSizeID = uitem.ProductSizeID,
                        SizeID = db.ProductSizes.Find(uitem.ProductSizeID).Size.SizeID,
                        SizeName = db.ProductSizes.Find(uitem.ProductSizeID).Size.SizeName,
                        OrderQTY = uitem.Quantity,
                        UnitPrice = uitem.UnitPrice,
                    };
                    var inactivity = db.ActivityProducts.Where(a => a.ProductID == uitem.ProductID).ToList();
                    if (inactivity.Count() > 0)
                    {
                        cartItem.SUnitPrice = Convert.ToInt32(inactivity.First().Activity.DiscountMethod.Discount * uitem.UnitPrice);
                        cartItem.DiscountID = inactivity.First().Activity.DiscountID;
                        cartItem.ActivityName = inactivity.First().Activity.ActivityName;
                    }
                    cart.Add(cartItem);
                }
                return cart;
            }
        }
    }
}