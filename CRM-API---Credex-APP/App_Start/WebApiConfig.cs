using CRM_API___Credex.Filters;
using CRM_API___Credex.MessageHandler;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CRM_API___Credex.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.MessageHandlers.Add(new MessageLoggingHandler());

            //config.Routes.MapHttpRoute(
            //    name: "Swagger UI",
            //    routeTemplate: "APIHelp",
            //    defaults: null,
            //    constraints: null,
            //    handler: new RedirectHandler(message => message.RequestUri.ToString(), "index"));

            config.Filters.Add(new BasicAuthenticationAttribute());
        }
    }
}