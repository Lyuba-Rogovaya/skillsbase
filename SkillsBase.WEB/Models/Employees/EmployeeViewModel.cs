﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsBase.WEB.Models.Employees
{
    public class EmployeeViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Profession { get; set; }
    }
}