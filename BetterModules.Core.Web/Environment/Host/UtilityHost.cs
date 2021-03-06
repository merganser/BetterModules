﻿using System;
using System.Web;
using System.Web.Routing;
using Autofac;
using BetterModules.Core.Dependencies;
using BetterModules.Core.Web.Environment.Application;
using BetterModules.Core.Web.Environment.Host;
using BetterModules.Core.Web.Modules.Registration;
using BetterModules.Events;

//[assembly : WebApplicationHost(typeof(UtilityHost), Order = Int32.MaxValue)]
namespace BetterModules.Core.Web.Environment.Host
{
    public sealed class UtilityHost : DefaultWebApplicationAutoHost
    {
        public override void Init(HttpApplication context)
        {
            Application = context;
            lock (Lock)
            {
                if (InitializedCount++ == 0)
                {
                    WebCoreEvents.Instance.OnHostStart(context);

                    using (var container = ContextScopeProvider.CreateChildContainer())
                    {
                        var modulesRegistration = container.Resolve<IWebModulesRegistration>();
                        modulesRegistration.RegisterKnownModuleRoutes(RouteTable.Routes);
                    }
                }
            }
        }

        public override void Dispose()
        {
            lock (Lock)
            {
                if (--InitializedCount == 0)
                {
                    WebCoreEvents.Instance.OnHostStop(Application);
                }
            }
        }
    }
}
