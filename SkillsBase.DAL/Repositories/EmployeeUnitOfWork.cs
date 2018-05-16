using Microsoft.AspNet.Identity;
using SkillsBase.DAL.EF;
using SkillsBase.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.DAL.Repositories
{
    public class EmployeeUnitOfWork: BaseUnitOfWork
    {
        private GenericRepository<UserProfileSkillsEntity> userSkills;
        private GenericRepository<UserProfileEntity> profile;
        private GenericRepository<ApplicationUser> user;

        public EmployeeUnitOfWork(SkillsBaseContext context) : base(context)
        {
        }
        public GenericRepository<UserProfileSkillsEntity> UserProfileSkills
        {
            get
            {
                if (userSkills == null)
                {
                    userSkills = new GenericRepository<UserProfileSkillsEntity>(db);
                }
                return userSkills;
            }
        }
        public GenericRepository<UserProfileEntity> Profile
        {
            get
            {
                if (profile == null)
                {
                    profile = new GenericRepository<UserProfileEntity>(db);
                }
                return profile;
            }
        }

        public GenericRepository<ApplicationUser> User
        {
            get
            {
                if (user == null)
                {
                    user = new GenericRepository<ApplicationUser>(db);
                }
                return user;
            }
        }
    }
}
