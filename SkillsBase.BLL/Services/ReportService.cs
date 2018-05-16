using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Models;
using SkillsBase.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly DomainUnitOfWork domainUnitOfWork;
        /// <summary>
        /// Provides functionality for reteiving statistical data based on users, skills and domains. 
        /// </summary>
        public ReportService(DomainUnitOfWork domainUnitOfWork)
        {
            if (domainUnitOfWork == null)
                throw new ArgumentNullException(nameof(domainUnitOfWork));

            this.domainUnitOfWork = domainUnitOfWork;
        }

        /// <summary>
        /// Provides statistical data based user skills and domains and aggregated information about other users. 
        /// </summary>
        public async Task<UserStatistics> GetUserStatistics(string userId)
        {
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentException("User id must be set.");

            UserStatistics result = new UserStatistics();
            IEnumerable<UserSkillInfo> currentUserSkills = null;
            IEnumerable<UserDomainInfo> currentUserDomains = null;
            IEnumerable<UserSkillInfo> otherUserSkills = null;
            IEnumerable<UserDomainInfo> otherUserDomains = null;

            await Task.Run(() => {
                currentUserSkills = (from d in domainUnitOfWork.UserProfileSkills.GetAll()
                                     join s in domainUnitOfWork.Skill.GetAll() on d.SkillId equals s.Id
                                     where d.UserProfileID == userId && d.SkillLevel != (int)SkillLevel.None
                                     select new UserSkillInfo() { SkillName = s.Name, SkillLevel = d.SkillLevel }).ToList();

            });

            await Task.Run(() => {
                currentUserDomains = (from s in domainUnitOfWork.UserProfileSkills.GetAll()
                                      join d in domainUnitOfWork.Domain.GetAll() on s.DomainId equals d.Id
                                      where s.UserProfileID == userId && s.SkillLevel != (int)SkillLevel.None
                                      group s by new { s.DomainId, d.Name } into g
                                      select new UserDomainInfo() { DomainName = g.Key.Name, SkillLevelSum = g.Sum(m => m.SkillLevel) }).ToList();
            });

            await Task.Run(() => {
                otherUserSkills = (  from d in domainUnitOfWork.UserProfileSkills.GetAll()
                                     join s in domainUnitOfWork.Skill.GetAll() on d.SkillId equals s.Id
                                     where d.UserProfileID != userId && d.SkillLevel != (int)SkillLevel.None
                                     group d by new { d.SkillId, s.Name  } into g
                                     select new UserSkillInfo() { SkillName = g.Key.Name, SkillLevel = g.Sum(x => x.SkillLevel) } ).ToList();
            });

            await Task.Run(() => {
                otherUserDomains = (from s in domainUnitOfWork.UserProfileSkills.GetAll()
                                      join d in domainUnitOfWork.Domain.GetAll() on s.DomainId equals d.Id
                                      where s.UserProfileID != userId &&  s.SkillLevel != (int)SkillLevel.None
                                      group s by new { s.DomainId, d.Name } into g
                                      select new UserDomainInfo() { DomainName = g.Key.Name, SkillLevelSum = g.Sum(m => m.SkillLevel) }).ToList();
            });

            if (currentUserSkills == null)
                currentUserSkills = new List<UserSkillInfo>();

            if (currentUserDomains == null)
                currentUserDomains = new List<UserDomainInfo>();

            if (otherUserSkills == null)
                otherUserSkills = new List<UserSkillInfo>();

            if (otherUserDomains == null)
                otherUserDomains = new List<UserDomainInfo>();

            result.CurrentUserSkillInfo = currentUserSkills;
            result.CurrentUserDomainInfo = currentUserDomains;
            result.OtherUserSkillInfo = otherUserSkills;
            result.OtherUserDomainInfo = otherUserDomains;

            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    domainUnitOfWork.Dispose();
                }
                this.disposed = true;
            }
        }
    }
}
