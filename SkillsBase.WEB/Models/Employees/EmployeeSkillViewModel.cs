using SkillsBase.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsBase.WEB.Models.Employees
{
    public class EmployeeSkillViewModel
    {
        public int SkillId { get; set; }
        public int DomainId { get; set; }
        public string Level { get; set; }
        public Employee Employee { get; set; }
    }
}