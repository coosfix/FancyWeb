using System.Web.Mvc;

namespace FancyWeb.Areas.CheckProcess
{
    public class CheckProcessAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CheckProcess";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CheckProcess_default",
                "CheckProcess/{controller}/{action}/{id}",
                new { action = "Detail", id = UrlParameter.Optional }
            );
        }
    }
}