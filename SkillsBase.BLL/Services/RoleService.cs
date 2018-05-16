using Microsoft.AspNet.Identity.EntityFramework;
using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Exceptions;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Models;
using SkillsBase.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Services
{
    public class RoleService: IService<Role>
    {
        private readonly IdentityUnitOfWork identityUnitOfWork;
        /// <summary>
        /// Provides CRUD functionality for roles. 
        /// </summary>
        public RoleService(IdentityUnitOfWork identityUnitOfWork)
        {
            this.identityUnitOfWork = identityUnitOfWork ?? throw new ArgumentNullException(nameof(identityUnitOfWork));
        }
        /// <summary>
        /// Lists all roles.
        /// </summary>
        public async Task<IEnumerable<Role>> ListAllAsync()
        {
            IEnumerable<Role> roles = null;
            await Task.Run(() => roles = identityUnitOfWork.RoleManager.Roles.Select(r => new Role() { Id = r.Id, Name = r.Name, Users = r.Users.Select(u => new UserDTO() { Id = u.UserId }) }));

            if (roles == null)
                roles = new List<Role>();

            return roles;
        }
        /// <summary>
        /// Creates Role.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RoleManagementException"></exception>
        public async Task SaveAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (String.IsNullOrEmpty(role.Name))
            {
                throw new RoleManagementException("Cannot create role with empty name.");
            }

            IdentityRole dbRole = await identityUnitOfWork.RoleManager.FindByNameAsync(role.Name);

            if (dbRole != null)
            {
                throw new RoleManagementException("Role with such name aleady exists.");
            }

            await identityUnitOfWork.RoleManager.CreateAsync(new IdentityRole() { Name = role.Name });
        }
        /// <summary>
        /// Find role by id.
        /// </summary>
        /// <exception cref="RoleManagementException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Role> FindAsync(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            var roleQuery =  from r in identityUnitOfWork.RoleManager.Roles
                        where r.Id.Equals(id)
                        select new Role()
                        {
                            Id = r.Id,
                            Name = r.Name
                        };

            var userQuery = from u in identityUnitOfWork.UserManager.Users
                        where u.Roles.Select(r2 => r2.RoleId).Contains(id)
                        select new UserDTO() { Id = u.Id, UserName = u.UserName, Email = u.Email };

            Role role = await roleQuery.FirstOrDefaultAsync();

            if (role != null)
                role.Users = await userQuery.ToListAsync();

            return role;
        }

        /// <summary>
        /// Deletes role.
        /// </summary>
        /// <exception cref="RoleManagementException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task DeleteAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            IdentityRole dbRole = await identityUnitOfWork.RoleManager.FindByIdAsync(role.Id);

            if (dbRole == null)
                throw new RoleManagementException("Role does not exist.");

            if (dbRole.Users.Count() > 0)
            {
                throw new RoleManagementException("Cannot delete role with users assigned to it.");
            }
            await identityUnitOfWork.RoleManager.DeleteAsync(dbRole);
        }
        /// <summary>
        /// Finds roles by search string if any (returns all if search string is null). 
        /// Returns subset of items if start and length parameters are specified (can be used for pagination). 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<GridSearchRestltDTO<Role>> FindByCriteriaAsync(string search, int start, int length)
        {
            if (start < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }
            int filteredResultsCount = 0;
            int totalResultsCount = 0;

            IEnumerable<Role> result = null;
            IQueryable<Role> query = identityUnitOfWork.RoleManager.Roles.Select(r => new Role() { Id = r.Id, Name = r.Name, Users = r.Users.Select(u => new UserDTO() { Id = u.UserId }) });

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(e => (
                    e.Name.Contains(search)
                ));
            }

            filteredResultsCount = query.Count();
            totalResultsCount = identityUnitOfWork.RoleManager.Roles.Count();

            query = query.OrderBy(q => q.Id);

            if (length > 0)
            {
                query = query.Skip(start).Take(length);
            }
            result = await query.ToListAsync();

            if (result == null)
            {
                result = new List<Role>();
            }

            return new GridSearchRestltDTO<Role>
            {
                FilteredResultsCount = filteredResultsCount,
                TotalResultsCount = totalResultsCount,
                Items = result
            };
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
                    identityUnitOfWork.Dispose();
                }
                this.disposed = true;
            }
        }

        public Task<Role> FindAsync(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
