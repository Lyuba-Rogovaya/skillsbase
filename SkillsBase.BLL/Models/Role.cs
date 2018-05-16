using SkillsBase.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Models
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<UserDTO> Users { get; set; }
    }
}
