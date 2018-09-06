using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWork.Exceptions
{
    public class NotEnoughMoneyException : ArgumentException
    {
        public NotEnoughMoneyException()
        {
        }

        public NotEnoughMoneyException(string message) : base(message)
        {
        }

        public NotEnoughMoneyException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}
