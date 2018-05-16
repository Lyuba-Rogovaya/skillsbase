using SkillsBase.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.DTO
{
    public class EmployeeSkillDTO
    {
        public int SkillId { get; set; }
        public int DomainId { get; set; }
        public string SkillName { get; set; }
        public string DomainName { get; set; }
        public SkillLevel Level { get; set; }
        public Employee Employee { get; set; }
    }
}
