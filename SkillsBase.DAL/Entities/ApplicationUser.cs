using Microsoft.AspNet.Identity.EntityFramework;

namespace SkillsBase.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public virtual UserProfileEntity UserProfile { get; set; }
    }
}
