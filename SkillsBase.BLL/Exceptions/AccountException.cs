using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Exceptions
{
    public class AccountException: Exception
    {
        public AccountException()
        {
        }

        public AccountException(string message)
            : base(message)
        {
        }

        public AccountException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
