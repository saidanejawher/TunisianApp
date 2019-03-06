using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Web.Mvc;
using Services;
using Services.Interface;

namespace TunisianApp.App_Start
{
    public class SimpleInjectorInitializer
    {
        public static void Initialize()
        {
            var container = new Container();
            container.Options.DefaultLifestyle = Lifestyle.Singleton;
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static void InitializeContainer(Container container)
        {
            container.Register<IClientsService, ClientsServices>();
            container.Register <IUserService, UserServices>();

            var concreteTypes = new List<Type>()
            {
                 typeof(ClientsServices),
                 typeof(UserServices)
          
            };

            container.Collection.Register<IUserService>(concreteTypes);
        }
    }
}