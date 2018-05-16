using System;

namespace SkillsBase.DAL.Entities
{
    public class UserProfileEntity
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string Profession { get; set; }

    }
}
