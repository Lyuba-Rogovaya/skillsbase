using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.AspNet.Identity;
using SkillsBase.BLL.Interfaces;
using System.Web.Mvc;
using Ninject;
using System;
using System.Configuration;
using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Models;

[assembly: OwinStartup(typeof(SkillsBase.WEB.App_Start.Startup))]

namespace SkillsBase.WEB.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            CreateKernel();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
            
        }


        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {

            var kernel = new StandardKernel();
            kernel.Load(AppDomain.CurrentDomain.GetAssemblies());

            string path = ConfigurationManager.AppSettings["DataDirectory"];
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            kernel.Settings.Set("DbDedaultConnection", "DefaultConnection");

            try
            {
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            DependencyResolver.SetResolver(new Utils.NinjectDependencyResolver(kernel));
        }
    }
}