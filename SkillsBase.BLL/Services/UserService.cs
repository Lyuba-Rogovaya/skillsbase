using Microsoft.AspNet.Identity.EntityFramework;
using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Exceptions;
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
    public class UserService: IService<User>, IUserService
    {
        private readonly IdentityUnitOfWork identityUnitOfWork;

        /// <summary>
        /// Provides functionality for listing users and user role management. 
        /// </summary>
        public UserService(IdentityUnitOfWork identityUnitOfWork)
        {
            if (identityUnitOfWork == null)
            {
                throw new ArgumentNullException(nameof(identityUnitOfWork));
            }
            this.identityUnitOfWork = identityUnitOfWork;
        }

        /// <summary>
        /// Find user by id.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<User> FindAsync(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            ApplicationUser appUser = await identityUnitOfWork.UserManager.FindByIdAsync(id);
            User user = null;
            if(appUser != null)
            {
                user = new User() { Id = appUser.Id, UserName = appUser.UserName, Email = appUser.Email, PhoneNumber = appUser.PhoneNumber };
            }
            return user;
        }

        public Task SaveAsync(User dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(User dto)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> ListAllAsync()
        {
            IEnumerable<User> result = null;
            await Task.Run(() => {
                IEnumerable<ApplicationUser> userList = identityUnitOfWork.UserManager.Users;
                if(userList != null)
                {
                    result = userList.Select(u => new User() { Id = u.Id, UserName = u.UserName, Email = u.Email, PhoneNumber = u.PhoneNumber });
                }
               
            });

            if (result == null)
                result = new List<User>();

            return result;
        }

        /// <summary>
        /// Finds users by search string if any (returns all if search string is null). 
        /// Returns subset of items if start and length parameters are specified (can be used for pagination). 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<GridSearchRestltDTO<User>> FindByCriteriaAsync(string search, int start, int length)
        {
            if (start < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }
            int filteredResultsCount = 0;
            int totalResultsCount = 0;

            IEnumerable<User> result = null;
            IQueryable<ApplicationUser> query = identityUnitOfWork.UserManager.Users;

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(e => (
                    e.UserName.Contains(search) ||
                    e.Email.Contains(search) ||
                    e.PhoneNumber.Contains(search)
                ));
            }

            filteredResultsCount = query.Count();
            totalResultsCount = identityUnitOfWork.UserManager.Users.Count();

            query = query.OrderBy(q => q.Id);

            if (length > 0)
            {
                query = query.Skip(start).Take(length);
            }
            result = await query.Select(u => new User() { Id = u.Id, UserName = u.UserName, Email = u.Email, PhoneNumber = u.PhoneNumber }).ToListAsync();

            if (result == null)
            {
                result = new List<User>();
            }

            return new GridSearchRestltDTO<User>
            {
                FilteredResultsCount = filteredResultsCount,
                TotalResultsCount = totalResultsCount,
                Items = result
            };
        }

        /// <summary>
        /// Adds user to role.  
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UserManagementException"></exception>
        public async Task AddUserToRoleAsync(string roleName, string userId)
        {
            if (String.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            try
            {
                await identityUnitOfWork.UserManager.AddToRoleAsync(userId, roleName);
            }
            catch(Exception ex)
            {
                throw new UserManagementException(ex.Message, ex);
            }
        }
        /// <summary>
        /// Removes user from role.  
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UserManagementException"></exception>
        public async Task RemoveUserFromRoleAsync(string roleName, string userId)
        {
            if (String.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            try
            {
                await identityUnitOfWork.UserManager.RemoveFromRoleAsync(userId, roleName);
            }
            catch (Exception ex)
            {
                throw new UserManagementException(ex.Message, ex);
            }
        }
        /// <summary>
        /// List user roles.  
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<IEnumerable<Role>> GetUserRolesAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            IEnumerable<Role> roles = null;

            IEnumerable<IdentityRole> dbRoles = await identityUnitOfWork.RoleManager.Roles.Where(r => r.Users.Select(u => u.UserId).Contains(userId)).ToListAsync();

            if (dbRoles != null)
            {
                roles = dbRoles.Select(r => new Role() { Id = r.Id, Name = r.Name });
            } else
            {
                roles = new List<Role>();
            }
            return roles;
        }

        public Task<User> FindAsync(int Id)
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
                    identityUnitOfWork.Dispose();
                }
                this.disposed = true;
            }
        }
    }
}
