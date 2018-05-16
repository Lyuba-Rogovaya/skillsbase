using SkillsBase.DAL.EF;
using SkillsBase.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.DAL.Repositories
{
    public class DomainUnitOfWork: BaseUnitOfWork
    {
        private GenericRepository<DomainEntity> domain;
        private GenericRepository<SkillEntity> skill;
        private GenericRepository<UserProfileSkillsEntity> userSkills;

        public DomainUnitOfWork(SkillsBaseContext context): base(context)
        {
        }
        public virtual GenericRepository<DomainEntity> Domain
        {
            get
            {
                if (domain == null)
                {
                    domain = new GenericRepository<DomainEntity>(db);
                }
                return domain;
            }
        }
        public virtual GenericRepository<SkillEntity> Skill
        {
            get
            {
                if (skill == null)
                {
                    skill = new GenericRepository<SkillEntity>(db);
                }
                return skill;
            }
        }
        public virtual GenericRepository<UserProfileSkillsEntity> UserProfileSkills
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
    }
}
