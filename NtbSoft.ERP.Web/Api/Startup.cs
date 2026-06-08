using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(NtbSoft.ERP.Web.Api.Startup))]

namespace NtbSoft.ERP.Web.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true,
                EnableJSONP = false,     
                EnableJavaScriptProxies = true
            };

            app.MapSignalR(hubConfiguration);
        }
    }
}
