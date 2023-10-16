using System.Web.Mvc;
using System.Web.Routing;

namespace Scorecraft.Api
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "ApiDefault",
            //    url: "api/{controller}/{action}/{id}",
            //    defaults: new { id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "WebDefault",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
