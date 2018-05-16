using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SkillsBase.DAL.EF;
using SkillsBase.DAL.Entities;
using System;

namespace SkillsBase.DAL.Repositories
{
    public class IdentityUnitOfWork : BaseUnitOfWork
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;

        public IdentityUnitOfWork(RoleManager<IdentityRole> rm, UserManager<ApplicationUser> um, SkillsBaseContext context) : base(context)
        {
            if (rm == null)
                throw new ArgumentNullException(nameof(rm));
            if (um == null)
                throw new ArgumentNullException(nameof(um));

            roleManager = rm;
            userManager = um;
        }

        public UserManager<ApplicationUser> UserManager
        {
            get { return userManager; }
        }


        public RoleManager<IdentityRole> RoleManager
        {
            get { return roleManager; }
        }
    }
}
