using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Exceptions;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Models;
using SkillsBase.DAL.Entities;
using SkillsBase.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IdentityUnitOfWork identityUnitOfWork;
        private readonly EmployeeUnitOfWork employeeUnitOfWork;
        /// <summary>
        /// Provides functionality for registering, authenticating, updating user and role management.
        /// </summary>
        public AccountService(IdentityUnitOfWork identityUnitOfWork, EmployeeUnitOfWork employeeUnitOfWork)
        {
            if (identityUnitOfWork == null)
                throw new ArgumentNullException(nameof(identityUnitOfWork));

            if (employeeUnitOfWork == null)
                throw new ArgumentNullException(nameof(employeeUnitOfWork));

            this.identityUnitOfWork = identityUnitOfWork;
            this.employeeUnitOfWork = employeeUnitOfWork;
        }

        /// <summary>
        /// Creates user, if it does not exist.
        /// Adds a role to the created user and creates user profile.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AccountException"></exception>
        public async Task Create(UserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            ApplicationUser user = await identityUnitOfWork.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.UserName };
                await identityUnitOfWork.UserManager.CreateAsync(user, userDto.Password);
                
                // add role
                await identityUnitOfWork.UserManager.AddToRoleAsync(user.Id, userDto.Role);

                // create user profile
                UserProfileEntity userProfile = new UserProfileEntity { Id = user.Id };
                employeeUnitOfWork.Profile.Insert(userProfile);

                await identityUnitOfWork.SaveAsync();
                await employeeUnitOfWork.SaveAsync();
            }
            else
            {
                throw new AccountException("User with such email already exists.");
            }
        }

        /// <summary>
        /// Authenticates user.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AccountException"></exception>
        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            ClaimsIdentity claim = null;

            ApplicationUser user = await identityUnitOfWork.UserManager.FindAsync(userDto.Email, userDto.Password);

            if (user != null)
            {
                claim = await identityUnitOfWork.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                return claim;
            }
            else
            {
                throw new AccountException("Invalid login or password, or user does not exist.");
            }
        }
        /// <summary>
        /// Updates profile.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task UpdateProfile(ProfileDTO dto)
        {
            if(dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (String.IsNullOrEmpty(dto.Id))
                throw new ArgumentNullException(nameof(dto.Id));

            UserProfileEntity profile = 
                new UserProfileEntity { Id = dto.Id, FirstName = dto.FirstName, LastName = dto.LastName, Profession = dto.Profession, Age = dto.Age };

            employeeUnitOfWork.Profile.Update(profile);

            await employeeUnitOfWork.SaveAsync();
        }
        /// <summary>
        /// Finds profile by user id.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ProfileDTO> GetProfile(string userId)
        {
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            ProfileDTO dto = null;
            UserProfileEntity profile = await employeeUnitOfWork.Profile.GetByID(userId);
            if(profile != null)
            {
                dto = new ProfileDTO { Id = profile.Id, Age = profile.Age, FirstName = profile.FirstName, LastName = profile.LastName, Profession = profile.Profession };
            }
            return dto;
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

