using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ex3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("location", "location/{ip}/{port}",
                defaults: new { controller = "Home", action = "GetLocation" });

            routes.MapRoute("info", "info/{ip}/{port}",
                defaults: new { controller = "Home", action = "GetInfo" });

            routes.MapRoute("saveAnimation", "save/{ip}/{port}/{updateInterval}/{animationTime}/{filename}",
                defaults: new { controller = "Home", action = "saveAnimation" });

            routes.MapRoute("saveFile", "saveFile/{filename}",
                defaults: new { controller = "Home", action = "saveFile" });

            routes.MapRoute("loadFile", "loadFile/{filename}",
                defaults: new { controller = "Home", action = "loadFile" });

            routes.MapRoute("display", "display/{ipOrFilename}/{portOrUpdateInterval}",
                defaults: new { controller = "Home", action = "display" });

            routes.MapRoute("displayPath", "display/{ip}/{port}/{updateInterval}",
                 defaults: new { controller = "Home", action = "displayPath" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
