using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Interfaces
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Finds all skills associated with current user.  
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        Task<IEnumerable<EmployeeSkillDTO>> GetEmployeeSkills(string userId);
        /// <summary>
        /// Updates user skill level in the db if user skill exists, otherwise inserts user skill.  
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        Task SaveSkillLevel(EmployeeSkillDTO employeeSkillDto, string userId);
        /// <summary>
        /// Finds employees by skill and skill level(s).   
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        Task<IEnumerable<EmployeeSkillDTO>> FindEmployeesBySkill(int domainId, int skillId, SkillLevel[] level);
    }
}
