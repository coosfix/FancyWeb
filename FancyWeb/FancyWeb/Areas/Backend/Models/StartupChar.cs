using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FancyWeb.Areas.Backend.Models.StartupChar))]

namespace FancyWeb.Areas.Backend.Models
{
    public class StartupChar
    {
        public void Configuration(IAppBuilder app)
        {
            // 如需如何設定應用程式的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkID=316888
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}
