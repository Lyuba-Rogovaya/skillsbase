using Microsoft.AspNet.Identity.EntityFramework;
using SkillsBase.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Interfaces
{
    public interface IUserService
    {
        Task AddUserToRoleAsync(string roleName, string userId);
        Task RemoveUserFromRoleAsync(string roleName, string userId);
        Task<IEnumerable<Role>> GetUserRolesAsync(string userId);
    }
}
