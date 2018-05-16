using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Models;
using SkillsBase.DAL.Entities;
using SkillsBase.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Services
{
    public class EmployeeService : IService<Employee>, IEmployeeService
    {
        private readonly DomainUnitOfWork domainUnitOfWork;
        private readonly EmployeeUnitOfWork employeeUnitOfWork;
        private readonly IdentityUnitOfWork identityUnitOfWork;

        /// <summary>
        /// Provides functionality to list employees, find employees by search criteria, update employee related data. 
        /// </summary>
        public EmployeeService(DomainUnitOfWork domain, EmployeeUnitOfWork employee, IdentityUnitOfWork identity)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));

            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            domainUnitOfWork = domain;
            employeeUnitOfWork = employee;
            identityUnitOfWork = identity;
        }
        /// <summary>
        /// Finds all skills associated with current user.  
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<IEnumerable<EmployeeSkillDTO>> GetEmployeeSkills(string userId)
        {
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            IEnumerable<EmployeeSkillDTO> userSkills = null;

            await Task.Run(() => {
                userSkills = from d in domainUnitOfWork.Domain.GetAll()
                join s in domainUnitOfWork.Skill.GetAll() on d.Id equals s.DomainId
                join u in domainUnitOfWork.UserProfileSkills.GetAll().Where(i => i.UserProfileID == userId).Select(r => new { UserProfileId = r.UserProfileID, r.SkillId, r.DomainId, r.SkillLevel }) on s.Id equals u.SkillId into t
                from tr in t.DefaultIfEmpty(new { UserProfileId = userId, SkillId = 0, DomainId = 0, SkillLevel = 0 })
                select new EmployeeSkillDTO { SkillName = s.Name, DomainName = d.Name, SkillId = s.Id, DomainId = s.DomainId, Level = (SkillLevel)tr.SkillLevel };
            });

            if(userSkills == null)
            {
                userSkills = new List<EmployeeSkillDTO>();
            }

            return userSkills;
        }
        /// <summary>
        /// Finds employees by skill and skill level(s).   
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<EmployeeSkillDTO>> FindEmployeesBySkill(int domainId, int skillId, SkillLevel[] level)
        {
            if (skillId <= 0)
                throw new ArgumentException("Skill id must be garter than zero.");

            if (domainId <= 0)
                throw new ArgumentException("Domain id must be garter than zero.");

            if(level == null || level.Count() == 0)
                throw new ArgumentException("Skill level must be set.");

            IEnumerable<EmployeeSkillDTO> employees = null;

            await Task.Run(() =>
            {
                employees = from u in employeeUnitOfWork.User.GetAll()
                            join s in employeeUnitOfWork.UserProfileSkills.GetAll().Where(s => s.SkillId == skillId).Select( x => new { x.DomainId, x.SkillId, x.SkillLevel, x.UserProfileID }) on u.Id equals s.UserProfileID into t
                            from tr in t.DefaultIfEmpty(new { DomainId = domainId, SkillId = skillId, SkillLevel = (int)SkillLevel.None, UserProfileID = u.Id })
                            where level.Contains((SkillLevel)tr.SkillLevel)
                            select new EmployeeSkillDTO
                            {   Employee = new Employee()
                                {
                                    Id = u.Id,
                                    UserName = u.UserName,
                                    FullName = u.UserProfile.FirstName + " " + u.UserProfile.LastName,
                                    Age = u.UserProfile.Age,
                                    Profession = u.UserProfile.Profession != null ? u.UserProfile.Profession : "not set",
                                    Email = u.Email,
                                    PhoneNumber = u.PhoneNumber != null ? u.PhoneNumber: "not set"
                                },
                                DomainId = tr.DomainId, SkillId = tr.SkillId, Level = (SkillLevel)tr.SkillLevel
                            };
            });

            if (employees == null)
                employees = new List<EmployeeSkillDTO>();

            return employees;
        }
        /// <summary>
        /// Updates user skill level in the db if user skill exists, otherwise inserts user skill.  
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task SaveSkillLevel(EmployeeSkillDTO employeeSkillDto, string userId)
        {
            if (employeeSkillDto == null)
                throw new ArgumentNullException(nameof(employeeSkillDto));
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            UserProfileSkillsEntity skill = await employeeUnitOfWork.UserProfileSkills.GetByID(employeeSkillDto.SkillId, userId);
            if (skill != null)
            {
                skill.SkillLevel = (int)employeeSkillDto.Level;
            }
            else
            {
                skill = new UserProfileSkillsEntity { DomainId = employeeSkillDto.DomainId, SkillId = employeeSkillDto.SkillId, SkillLevel = (int)employeeSkillDto.Level, UserProfileID = userId };
                employeeUnitOfWork.UserProfileSkills.Insert(skill);
            }
            await employeeUnitOfWork.SaveAsync();
        }

        public Task DeleteAsync(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<Employee> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds employees by search string if any (returns all if search string is null). 
        /// Returns subset of items if start and length parameters are specified (can be used for pagination).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<GridSearchRestltDTO<Employee>> FindByCriteriaAsync(string search, int start, int length)
        {
            if (start < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }
            int filteredResultsCount = 0;
            int totalResultsCount = 0;

            IEnumerable<Employee> result = null;
            IQueryable<Employee> query = employeeUnitOfWork.Profile.GetAll().Join(
                    employeeUnitOfWork.User.GetAll(),
                    e => e.Id,
                    u => u.Id,
                    (e, u) => new Employee { Id = e.Id, UserName = u.UserName, FullName = e.FirstName + " " + e.LastName, Age = e.Age, Profession = e.Profession, Email = u.Email, PhoneNumber = u.PhoneNumber }
                );

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(e => (
                    e.UserName.Contains(search) ||
                    e.FullName.Contains(search) ||
                    e.Profession.Contains(search) ||
                    e.Email.Contains(search) ||
                    e.PhoneNumber.Contains(search) ||
                    e.Age.ToString().Contains(search)
                ));
            }

            filteredResultsCount = query.Count();
            totalResultsCount = employeeUnitOfWork.Profile.Count();

            query = query.OrderBy(q => q.Id);

            if (length > 0)
            {
                query = query.Skip(start).Take(length);
            }
            result = await query.ToListAsync();

            if (result == null)
            {
                result = new List<Employee>();
            }

            return new GridSearchRestltDTO<Employee>
            {
                FilteredResultsCount = filteredResultsCount,
                TotalResultsCount = totalResultsCount,
                Items = result
            };
        }
        public async Task<IEnumerable<Employee>> ListAllAsync()
        {
            IEnumerable<Employee> employees = null;
            await Task.Run(() => {
                employees = employeeUnitOfWork.Profile.GetAll().Join(
                    employeeUnitOfWork.User.GetAll(),
                    e => e.Id,
                    u => u.Id,
                    (e, u) => new Employee { Id = e.Id, FullName = e.FirstName + ' ' + e.LastName, Age = e.Age, Profession = e.Profession, Email = u.Email, PhoneNumber = u.PhoneNumber }
                ).ToList();
            }); 

            if(employees == null)
            {
                employees = new List<Employee>();
            }

            return employees;
        }

        public Task SaveAsync(Employee dto)
        {
            throw new NotImplementedException();
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
                    employeeUnitOfWork.Dispose();
                    identityUnitOfWork.Dispose();
                }
                this.disposed = true;
            }
        }

        public async Task<Employee> FindAsync(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
