using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Models
{
    public class UserStatistics
    {
        public IEnumerable<UserSkillInfo> CurrentUserSkillInfo { get; set; }
        public IEnumerable<UserDomainInfo> CurrentUserDomainInfo { get; set; }
        public IEnumerable<UserSkillInfo> OtherUserSkillInfo { get; set; }
        public IEnumerable<UserDomainInfo> OtherUserDomainInfo { get; set; }
    }
}
