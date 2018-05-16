using System;

namespace SkillsBase.DAL.Entities
{
    public class UserProfileSkillsEntity
    {
        public string UserProfileID { get; set; }
        public int SkillId { get; set; }
        public int SkillLevel { get; set; }
        public int DomainId { get; set; }
        public virtual DomainEntity Domain { get; set; }
    }
}
