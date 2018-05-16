
using SkillsBase.BLL.Models;
using System;
using System.Collections.Generic;
using SkillsBase.BLL.Exceptions;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Interfaces
{
    public interface IDomainService
    {
        /// <summary>
        /// Removes skills from the db.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DomainException"></exception>
        Task RemoveSkill(int skillId);
        /// <summary>
        /// Inserts new skills into the db.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DomainException"></exception>
        Task<int> AddSkill(Skill skill);
        /// <summary>
        /// Finds skills associated with specified domain.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        Task<IEnumerable<Skill>> GetSkills(int domainId);
    }
}
