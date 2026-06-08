using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;

namespace NtbSoft.ERP.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            // v2.5.0 — Force JSON formatter to use UTF-8 without BOM
            // Fixes Vietnamese text garbling (UTF-8 bytes read as Latin-1)
            config.Formatters.JsonFormatter.SupportedMediaTypes.Clear();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            config.Formatters.JsonFormatter.SupportedEncodings.Clear();
            config.Formatters.JsonFormatter.SupportedEncodings.Add(new UTF8Encoding(false)); // no BOM
            // Ensure Vietnamese unicode chars are NOT escaped to \uXXXX
            var jsonSettings = config.Formatters.JsonFormatter.SerializerSettings;
            jsonSettings.StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.Default;

            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
               name: "ApiById",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional },
               constraints: new { id = @"^[0-9]+$" }
            );

            config.Routes.MapHttpRoute(
              name: "ApiByActionName",
              routeTemplate: "api/{controller}/{action}/{name}",
              defaults: new { id = RouteParameter.Optional },
              constraints: new { action = @"^[a-z]+$", name = @"^[a-z]+$" }
           );

            config.Routes.MapHttpRoute(
                name: "ApiByName",
                routeTemplate: "api/{controller}/{action}/{name}",
                defaults: null,
                constraints: new { name = @"^[a-z]+$" }
            );

            config.Routes.MapHttpRoute(
                name: "ApiByAction",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { action = "Get" }
            );

        }
    }
}
