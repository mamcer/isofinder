using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IsoFinder.Application;
using IsoFinder.Data;

namespace IsoFinder.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            var userName = HttpContext.Current.User.Identity.Name;
            var isoUserService = new IsoUserService(new UnitOfWork());
            var user = isoUserService.GetByName(userName);
            if (user == null)
            {
                user = isoUserService.Insert(userName);
            }

            HttpContext.Current.Session["UserInfo"] = new Models.UserInfo { Id = user.Id, Name = user.Name };
        }
    }
}