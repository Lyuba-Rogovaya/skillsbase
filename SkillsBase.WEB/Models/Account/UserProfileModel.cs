using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkillsBase.WEB.Models.Account
{
    public class UserProfileModel
    {
        public string Id { get; set; }
        [Display(Name = "First Name")]
        [StringLength(250, ErrorMessage = "Maximum name length is 250 characters.")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [StringLength(250, ErrorMessage = "Maximum name length is 250 characters.")]
        public string LastName { get; set; }
        [Range(18, 65, ErrorMessage = "Age must be between 18 and 65.")]
        public int Age { get; set; }
        [StringLength(250, ErrorMessage = "Maximum name length is 250 characters.")]
        public string Profession { get; set; }
    }
}