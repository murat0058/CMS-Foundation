﻿using System;
using System.Web;
using Microsoft.Framework.DependencyInjection;

namespace Composite.Core.Application
{
    /// <summary>
    /// Build in service locator that is used for the injecting dependencies into c1 functions.
    /// </summary>
    public static class ServiceLocator
    {
        private const string HttpContextKey = "HttpApplication.ServiceScope";
        private static IServiceCollection _serviceCollection;
        private static IServiceProvider _applicationServices;


        public static IServiceCollection ServiceCollection
        {
            get
            {
                if (_serviceCollection == null)
                {
                    _serviceCollection = new ServiceCollection();
                }
                return _serviceCollection;
            } 
            set { _serviceCollection = value; }
        }

        
        /// <summary>
        /// Gets an application service provider
        /// </summary>
        public static IServiceProvider ApplicationServices 
        {
            get
            {
                if (_applicationServices == null)
                {
                    if (_serviceCollection == null) return null;

                    _applicationServices = _serviceCollection.BuildServiceProvider();
                }

                return _applicationServices;
            }
            set { _applicationServices = value; } 
        }


        /// <summary>
        /// Gets a request service provider
        /// </summary>
        public static IServiceProvider RequestServices
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null) return null;

                var scope = (IServiceScope)context.Items[HttpContextKey];
                return scope != null ? scope.ServiceProvider : null;
            }
        }

        /// <summary>
        /// Creates a service scope associated with the current http context
        /// </summary>
        public static void CreateRequestServicesScope()
        {
            if (ApplicationServices == null)
            {
                return;
            }

            var serviceScopeFactory = (IServiceScopeFactory) ApplicationServices.GetService(typeof(IServiceScopeFactory));
            var serviceScope = serviceScopeFactory.CreateScope();

            var context = HttpContext.Current;
            context.Items[HttpContextKey] = serviceScope;
        }

        /// <summary>
        /// Disposes a service scope associated with the current http context
        /// </summary>
        public static void DisposeRequestServicesScope()
        {
            if (ApplicationServices == null)
            {
                return;
            }

            var context = HttpContext.Current;
            var scope = (IServiceScope)context.Items[HttpContextKey];

            if (scope != null)
            {
                scope.Dispose();
            }
        }
    }
}
