using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OrderManagement.Web.Startup))]
namespace OrderManagement.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}