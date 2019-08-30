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
                        cartItem.SUnitPrice = Convert.ToInt32(Math.Floor(inactivity.First().Activity.DiscountMethod.Discount * uitem.UnitPrice));
                        cartItem.DiscountID = inactivity.First().Activity.DiscountID;
                        cartItem.ActivityName = inactivity.First().Activity.ActivityName;
                    }
                    cart.Add(cartItem);
                }
                return cart;
            }
        }

        public static IQueryable<ProductCell> CreateProductCells(IQueryable<ProductColor> preproducts)
        {
            var products = preproducts.Select(p => new ProductCell
            //var products = preproducts.Where(p => p.Product.ProductInDate >= DateTime.Now && p.Product.ProductOutDate <= DateTime.Now).Select(p => new ProductCell
            {
                ProductID = p.Product.ProductID,
                ProductName = p.Product.ProductName,
                CategorySID = p.Product.CategorySID,
                ColorID = p.ColorID,
                SizeID = p.Product.ProductSizes.Select(s => s.SizeID).ToList(),
                R = p.Color.R,
                G = p.Color.G,
                B = p.Color.B,
                UnitPrice = p.Product.UnitPrice,
                SUnitPrice = p.Product.ActivityProducts.Where(a => a.ProductID == p.ProductID).Count() > 0 ? Math.Floor(p.Product.ActivityProducts.Where(a => a.ProductID == p.ProductID).FirstOrDefault().Activity.DiscountMethod.Discount * p.Product.UnitPrice) : p.Product.UnitPrice,
                ProductInDate = p.Product.ProductInDate,
                ActivityName = p.Product.ActivityProducts.FirstOrDefault().Activity.ActivityName
            });

            return products;
        }

        public static IQueryable<ProductCell> SetCellsByOrder(IQueryable<ProductCell> products, int orderid)
        {
            switch (orderid)
            {
                case 1:
                    products = products.OrderBy(o => o.SUnitPrice).ThenBy(o => o.ProductID);
                    break;
                case 2:
                    products = products.OrderByDescending(o => o.SUnitPrice).ThenBy(o => o.ProductID);
                    break;
                case 3:
                    products = products.OrderByDescending(o => o.ProductInDate).ThenBy(o => o.ProductID);
                    break;
                case 4:
                    products = products.OrderBy(o => o.ProductInDate).ThenBy(o => o.ProductID);
                    break;
            }
            return products;
        }
    }
}