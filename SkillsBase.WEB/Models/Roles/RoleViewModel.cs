
using SkillsBase.WEB.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkillsBase.WEB.Models.Roles
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Field is required.")]
        [StringLength(256, ErrorMessage = "Maximum name length is 256 characters.")]
        public string Name { get; set; }
        public IEnumerable<User> Users { get; set; }  
    }
}