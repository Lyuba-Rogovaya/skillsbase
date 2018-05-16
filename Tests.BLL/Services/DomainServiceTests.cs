using Moq;
using NUnit.Framework;
using SkillsBase.BLL.Exceptions;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Models;
using SkillsBase.BLL.Services;
using SkillsBase.DAL.EF;
using SkillsBase.DAL.Entities;
using SkillsBase.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.BLL.Services
{
    [TestFixture]
    public class DomainServiceTests
    {
        private IService<Domain> domainService;
        private Mock<DomainUnitOfWork> domainUnitOfWork;
        private Mock<SkillsBaseContext> context;
        private string dbConnection = "DefaultConnection";
        private Mock<GenericRepository<DomainEntity>> repositoryDomain;
        private Mock<GenericRepository<SkillEntity>> repositorySkill;
        private Mock<GenericRepository<UserProfileSkillsEntity>> repositoryUserProfileSkill;

        [SetUp]
        public void SetUp()
        {
            context = new Mock<SkillsBaseContext>(dbConnection);
            repositoryDomain = new Mock<GenericRepository<DomainEntity>>(context.Object);
            repositorySkill = new Mock<GenericRepository<SkillEntity>>(context.Object);
            repositoryUserProfileSkill = new Mock<GenericRepository<UserProfileSkillsEntity>>(context.Object);

            domainUnitOfWork = new Mock<DomainUnitOfWork>(context.Object);
            domainUnitOfWork.Setup(d => d.Domain).Returns(repositoryDomain.Object);
            domainUnitOfWork.Setup(d => d.Skill).Returns(repositorySkill.Object);
            domainUnitOfWork.Setup(d => d.UserProfileSkills).Returns(repositoryUserProfileSkill.Object);
            domainUnitOfWork.Setup(d => d.SaveAsync()).Returns(Task.Factory.StartNew(() => { }));

            domainService = new DomainService(domainUnitOfWork.Object);
        }


        [Test]
        public void SaveAsync_TryToSaveNullValue_ShouldThrow()
        {
            // act & assert
            Assert.ThrowsAsync<ArgumentNullException>(() => domainService.SaveAsync(null));
        }

        [Test]
        public void SaveAsync_TryToSaveDomainWithEmptyName_ShouldThrow()
        {
            // arrange
            Domain domain = new Domain() { Name = "" };

            // act & assert
            Assert.ThrowsAsync<DomainException>(() => domainService.SaveAsync(domain));
        }

        [Test]
        public void SaveAsync_SaveNotExistingDomainWithRandomId_ShouldThrow()
        {
            // arrange
            Domain domain = new Domain() { Name = "Test" };
            Random random = new Random();
            domain.Id = random.Next(100000, 500000);
            DomainEntity entity = null;
            repositoryDomain.Setup(m => m.GetByID(domain.Id)).Returns(Task.Factory.StartNew(() => entity));

            // act & assert
            Assert.ThrowsAsync<DomainException>(() => domainService.SaveAsync(domain));
        }

        [Test]
        public void SaveAsync_SaveDomainWithAssignedUsers_ShouldThrow()
        {
            // arrange
            Domain domain = new Domain() { Name = "Test", Id = 1 };
            repositoryDomain.Setup(m => m.GetByID(domain.Id)).Returns(Task.Factory.StartNew(() => 
                new DomainEntity() { Name = "Test", Id = 1, UserProfileSkills = new List<UserProfileSkillsEntity>() { new UserProfileSkillsEntity() } }));

            // act & assert
            Assert.That(() => domainService.SaveAsync(domain),
                Throws.TypeOf<DomainException>()
                .With.Message.EqualTo("Cannot update domain with assigned users."));
        }

        [Test]
        public void AddSkill_NullValueArgument_ShouldThrow()
        {
            // arrange
            var service = domainService as IDomainService;
            
            // act & assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.AddSkill(null));
        }

        [Test]
        public void AddSkill_SaveSkillWithEmptyName_ShouldThrow()
        {
            // arrange
            Skill skill = new Skill() { Name = "" };
            var service = domainService as IDomainService;
            
            // act & assert
            Assert.That(() => service.AddSkill(skill),
                Throws.TypeOf<DomainException>()
                .With.Message.EqualTo("Skill name must be set."));
        }
    }
}
