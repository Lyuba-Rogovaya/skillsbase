using System.Collections.Generic;

namespace SkillsBase.DAL.Entities
{
    public class DomainEntity
    {
        public int Id { get; set;}
        public string Name { get; set; }
        public string Description { get; set;}
        public virtual ICollection<UserProfileSkillsEntity> UserProfileSkills { get; set; }
        public virtual ICollection<SkillEntity> Skills { get; set; }
    }
}
