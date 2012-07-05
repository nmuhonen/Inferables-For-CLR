using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Inferables;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using SampleMVCApp.Controllers;

namespace SampleMVCApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // Override the default controller factory with the inferables aware factory.
            ControllerBuilder.Current.SetControllerFactory(new InferablesControllerFactory());
        }

        // This factory allows for binding uising inferables.
        class InferablesControllerFactory : DefaultControllerFactory
        {
            // use the commented line below to infer from the Mocks namespace path instead.

            private IModule module = ModuleManager.CreateModule("~");
            //private IModule module = ModuleManager.CreateModule("~.Mocks,~");

            protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
            {
                var returnContoller = controllerType != null
                    ? module.GetExplicit(controllerType)
                    : null;
                return (IController)returnContoller;
            }
        }

    }
}