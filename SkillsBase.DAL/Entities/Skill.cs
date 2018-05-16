using System;

namespace SkillsBase.DAL.Entities
{
    public class SkillEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DomainId { get; set; }
        public virtual DomainEntity Domain { get; set; }
    }
}
