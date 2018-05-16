using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using SkillsBase.DAL.EF;
using SkillsBase.DAL.Entities;

namespace SkillsBase.DAL.Infrastructure
{
    public class DALModule : NinjectModule
    {
        public DALModule()
        {
           
        }
        public override void Load()
        {
            string connectionString = Kernel.Settings.Get("DbDedaultConnection", "DefaultConnection");

            Bind<SkillsBaseContext>().ToSelf().InRequestScope().WithConstructorArgument(connectionString);

            Bind<IUserStore<ApplicationUser>>().To<UserStore<ApplicationUser>>().WithConstructorArgument("context",
        context => Kernel.Get<SkillsBaseContext>());
            Bind<UserManager<ApplicationUser>>().ToSelf();

            Bind<IRoleStore<IdentityRole, string>>().To<RoleStore<IdentityRole>>().WithConstructorArgument("context",
        context => Kernel.Get<SkillsBaseContext>());
            Bind<RoleManager<IdentityRole>>().ToSelf();

        }
    }
}
