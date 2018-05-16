using Ninject;
using Ninject.Modules;
using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Models;
using SkillsBase.BLL.Services;
using SkillsBase.DAL.EF;
using SkillsBase.DAL.Entities;
using SkillsBase.DAL.Repositories;

namespace SkillsBase.BLL.Infrastructure
{
    public class BLLModule : NinjectModule
    {
        public BLLModule()
        {

        }
        public override void Load()
        {
            Bind<IdentityUnitOfWork>().ToSelf();
            Bind<DomainUnitOfWork>().ToSelf();
            Bind<EmployeeUnitOfWork>().ToSelf();
            Bind<IAccountService>().To<AccountService>();
            Bind<IService<Domain>>().To<DomainService>();
            Bind<IService<Employee>>().To<EmployeeService>();
            Bind<IService<Role>>().To<RoleService>();
            Bind<IService<User>>().To<UserService>();
            Bind<IReportService>().To<ReportService>();
        }
    }
}
