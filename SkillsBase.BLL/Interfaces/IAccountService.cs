using Microsoft.AspNet.Identity.EntityFramework;
using SkillsBase.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SkillsBase.BLL.Exceptions;

namespace SkillsBase.BLL.Interfaces
{
    public interface IAccountService : IDisposable
    {
        /// <summary>
        /// Creates user, if it does not exist.
        /// Adds a role to the created user and creates user profile.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AccountException"></exception>
        Task Create(UserDTO userDto);
        /// <summary>
        /// Authenticates user.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AccountException"></exception>
        Task<ClaimsIdentity> Authenticate(UserDTO userDto);
        /// <summary>
        /// Updates profile.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        Task UpdateProfile(ProfileDTO dto);
        /// <summary>
        /// Finds profile by user id.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        Task<ProfileDTO> GetProfile(string userId);

    }
}
