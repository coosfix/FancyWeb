using System.Web.Mvc;

namespace FancyWeb.Areas.ProductDisplay
{
    public class ProductDisplayAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ProductDisplay";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProductDisplay_default",
                "ProductDisplay/{controller}/{action}/{id}",
                new { action = "Browse", id = UrlParameter.Optional }
            );
        }
    }
}