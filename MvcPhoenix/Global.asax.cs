﻿using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcPhoenix
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

        protected void Application_Error(object sender, EventArgs e)
        {
            //handle exceptions, send them via email or write to DB
            var exception = Server.GetLastError();
        }
    }
}