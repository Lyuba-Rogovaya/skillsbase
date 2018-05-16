using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DomainId { get; set; }
    }
}
