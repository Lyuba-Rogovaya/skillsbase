using Microsoft.AspNet.Identity.EntityFramework;
using SkillsBase.DAL.Entities;
using SkillsBase.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.DAL.EF
{
    public class SkillsBaseContext : IdentityDbContext<ApplicationUser>
    {
        public SkillsBaseContext(string conectionString) : base(conectionString)
        {
            if (String.IsNullOrEmpty(conectionString))
                throw new ArgumentNullException(nameof(conectionString));

            //Database.SetInitializer<SkillsBaseContext>(new ApplicationInitalizer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserProfileEntity>().HasRequired<ApplicationUser>(p => p.ApplicationUser);
            modelBuilder.Entity<UserProfileSkillsEntity>().HasKey(up => new { up.SkillId, up.UserProfileID});
            modelBuilder.Entity<DomainEntity>().Property(d => d.Name).HasMaxLength(250).IsRequired();
            modelBuilder.Entity<DomainEntity>().Property(d => d.Description).HasMaxLength(500);
            modelBuilder.Entity<SkillEntity>().Property(d => d.Name).HasMaxLength(250).IsRequired();
            modelBuilder.Entity<UserProfileEntity>().Property(d => d.FirstName).HasMaxLength(250);
            modelBuilder.Entity<UserProfileEntity>().Property(d => d.LastName).HasMaxLength(250);
            modelBuilder.Entity<UserProfileEntity>().Property(d => d.Age);
            modelBuilder.Entity<UserProfileEntity>().Property(d => d.Profession).HasMaxLength(250);

            base.OnModelCreating(modelBuilder);
        }
    }

    class ApplicationInitalizer : DropCreateDatabaseIfModelChanges<SkillsBaseContext>
    {
        protected override void Seed(SkillsBaseContext context)
        {
            List<IdentityRole> roles = new List<IdentityRole> { new IdentityRole { Name = "user" }, new IdentityRole { Name = "hr" }, new IdentityRole { Name = "admin" } };
            roles.ForEach(s => context.Roles.Add(s));

            context.SaveChanges();
        }

    }
}
