﻿using MyTend.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyTend.Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            this.InitData();
        }

        private void InitData()
        {
            var dbStarter = new DataBaseStarter();

            var entities = this.GetEntities();

            dbStarter.Add(entities);

            dbStarter.Initialize();
        }

        private Type[] GetEntities()
        {
            var list = new List<Type>();

            /*list.Add(typeof(City));
            list.Add(typeof(Country));
            list.Add(typeof(Image));
            list.Add(typeof(Region));
            list.Add(typeof(Tender));
            list.Add(typeof(TenderCategories));
            list.Add(typeof(TenderMessage));
            list.Add(typeof(TenderTheme));
            list.Add(typeof(UserMessage));
            list.Add(typeof(UserSystem));
            list.Add(typeof(PayInfo));*/

            return list.ToArray();
        }
    }
}
