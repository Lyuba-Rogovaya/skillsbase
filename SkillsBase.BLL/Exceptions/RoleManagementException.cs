using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Exceptions
{
    public class RoleManagementException : Exception
    {
        public RoleManagementException()
        {
        }

        public RoleManagementException(string message)
            : base(message)
        {
        }

        public RoleManagementException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
