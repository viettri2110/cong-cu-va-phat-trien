using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TMDT.Startup))]

namespace TMDT
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cấu hình SignalR
            app.MapSignalR();
        }
    }
}
