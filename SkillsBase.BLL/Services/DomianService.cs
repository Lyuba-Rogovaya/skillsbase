using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Exceptions;
using SkillsBase.BLL.Interfaces;
using SkillsBase.DAL.Repositories;
using SkillsBase.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using SkillsBase.BLL.Models;

namespace SkillsBase.BLL.Services
{
    public class DomainService: IService<Domain>, IDomainService
    {
        private readonly DomainUnitOfWork domainUnitOfWork;

        /// <summary>
        /// Provides CRUD functionality to domain and related skill set. 
        /// </summary>
        public DomainService(DomainUnitOfWork domainUnitOfWork)
        {
            if (domainUnitOfWork == null)
                throw new ArgumentNullException(nameof(domainUnitOfWork));

            this.domainUnitOfWork = domainUnitOfWork;
        }
        /// <summary>
        /// Update domain if it already exists, otherwise inserts domain.
        /// Inserts new skills if any.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DomainException"></exception>
        public async Task SaveAsync(Domain dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (String.IsNullOrEmpty(dto.Name) || String.IsNullOrWhiteSpace(dto.Name))
                throw new DomainException("Knowledge domian name must be set.");

            DomainEntity domain = null;

            if(dto.Id > 0)
            {
                domain = await domainUnitOfWork.Domain.GetByID(dto.Id);
                if(domain == null)
                {
                    throw new DomainException($"Could not find domain with id {dto.Id}.");
                }
                if (domain.UserProfileSkills.Count() > 0)
                {
                    throw new DomainException("Cannot update domain with assigned users.");
                }
                domain.Name = dto.Name;
                domain.Description = dto.Description;
                domainUnitOfWork.Domain.Update(domain);
            } else
            {
                domain = new DomainEntity { Name = dto.Name, Description = dto.Description };
                domainUnitOfWork.Domain.Insert(domain);
            }
            if(dto.Skills != null)
            {
                foreach(Skill s in dto.Skills)
                {
                    if(s.Id == 0)
                    {
                        if (String.IsNullOrEmpty(s.Name) || String.IsNullOrWhiteSpace(s.Name))
                            throw new DomainException("Skill name must be set.");

                        SkillEntity skill = new SkillEntity { Name = s.Name, DomainId = domain.Id };
                        domainUnitOfWork.Skill.Insert(skill);
                    }
                }
            }
            await domainUnitOfWork.SaveAsync();

        }
        /// <summary>
        /// Inserts new skills into the db.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DomainException"></exception>
        public async Task<int> AddSkill(Skill skill)
        {
            if (skill == null)
                throw new ArgumentNullException(nameof(skill));

            if (String.IsNullOrEmpty(skill.Name) || String.IsNullOrWhiteSpace(skill.Name))
                throw new DomainException("Skill name must be set.");

            SkillEntity s = new SkillEntity { Name = skill.Name, DomainId = skill.DomainId };
            domainUnitOfWork.Skill.Insert(s);
            await domainUnitOfWork.SaveAsync();
            return s.Id; 
        }
        /// <summary>
        /// Removes skills from the db.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DomainException"></exception>
        public async Task RemoveSkill(int skillId)
        {
            if(skillId <= 0)
            {
                throw new ArgumentException($"Invalid skill id. Id must be grater than zero.");
            }
            SkillEntity skillToRemove = await domainUnitOfWork.Skill.GetByID(skillId);
            if(skillToRemove == null)
            {
                throw new DomainException($"Could not find skill with id {skillId}.");
            }
            if (skillToRemove.Domain.UserProfileSkills.Where(s => (s.SkillId == skillId && s.SkillLevel != (int)SkillLevel.None)).Count() > 0)
            {
                throw new DomainException("Cannot remove skill beacause users are assigned to it.");
            }
            domainUnitOfWork.Skill.Delete(skillToRemove);
            await domainUnitOfWork.SaveAsync();
        }
        public async Task<IEnumerable<Domain>> ListAllAsync()
        {
            IEnumerable<Domain> result = null;
            var domains = await domainUnitOfWork.Domain.GetAll().OrderBy(t => t.Name).ToListAsync();
            if(domains != null)
            {
                result = domains.Select(d => new Domain { Id = d.Id, Name = d.Name, Description = d.Description });
            }
            else
            {
                result = new List<Domain>();
            }
            return result;
        }
        /// <summary>
        /// Finds domain by search string if any (returns all if search string is null). 
        /// Returns subset of items if start and length parameters are specified (can be used for pagination). 
        /// Preorders result set in specified order.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<GridSearchRestltDTO<Domain>> FindByCriteriaAsync(string search, int start, int length)
        {
            if(start < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }
            int filteredResultsCount = 0;
            int totalResultsCount = 0;
            
            IEnumerable<Domain> result = null;
            IQueryable<DomainEntity> query = domainUnitOfWork.Domain.GetAll();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(d => (d.Name.Contains(search) || d.Description.Contains(search)));
            }

            filteredResultsCount = query.Count();
            totalResultsCount = domainUnitOfWork.Domain.Count();

            query = query.OrderBy(q => q.Id);

            if (length > 0)
            {
                query = query.Skip(start).Take(length);
            }
            result = await query.Select(d => new Domain { Id = d.Id, Name = d.Name, Description = d.Description }).ToListAsync();
            if (result == null)
            {
                result = new List<Domain>();
            }
            
            return new GridSearchRestltDTO<Domain>
            {
                FilteredResultsCount = filteredResultsCount,
                TotalResultsCount = totalResultsCount,
                Items = result
            };
        }
        /// <summary>
        /// Finds domain by id.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DomainException"></exception>
        public async Task<Domain> FindAsync(int Id)
        {
            if (Id <= 0)
                throw new ArgumentException($"Invalid domain id. Id must be grater than zero.");

            DomainEntity dbDomain = await domainUnitOfWork.Domain.GetByID(Id);
            Domain domain = null;

            if (dbDomain != null)
            {
                IEnumerable<Skill> skills = dbDomain.Skills.Select( s => new Skill { Id = s.Id, Name = s.Name });
                domain = new Domain { Id = dbDomain.Id, Name = dbDomain.Name, Description = dbDomain.Description, Skills = skills };
            }

            return domain;

        }
        /// <summary>
        /// Removes domain from the db. Forbids deleting domain if there are usersers assigned to id.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DomainException"></exception>
        public async Task DeleteAsync(Domain dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            DomainEntity domain = await domainUnitOfWork.Domain.GetByID(dto.Id);
            if(domain == null)
            {
                throw new DomainException($"Could not find domain with id {dto.Id}");
            }
            if (domain.UserProfileSkills.Count() > 0)
            {
                throw new DomainException("Cannot delete domain with assigned users.");
            }
            domainUnitOfWork.Domain.Delete(domain);
            await domainUnitOfWork.SaveAsync();
        }
        /// <summary>
        /// Finds skills associated with specified domain.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<Skill>> GetSkills(int domainId)
        {
            if (domainId <= 0)
                throw new ArgumentException("Domain id cannot be less than or equal to zero.");

            return await domainUnitOfWork.Skill.GetAll().Where(s => s.DomainId == domainId).Select(s => new Skill() { Id = s.Id, DomainId = s.DomainId, Name = s.Name }).ToListAsync();
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

        public Task<Domain> FindAsync(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
