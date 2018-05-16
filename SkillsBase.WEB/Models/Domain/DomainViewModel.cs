using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkillsBase.WEB.Models.Domain
{
    public class DomainViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(250, ErrorMessage = "Maximum name length is 250 characters.")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Maximum description length is 500 characters.")]
        public string Description { get; set; }
        public IEnumerable<Skill> Skills {get; set;}
    }
}