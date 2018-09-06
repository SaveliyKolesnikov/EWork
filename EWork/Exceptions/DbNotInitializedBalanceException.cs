using System.Data.Common;

namespace EWork.Exceptions
{
    public class DbNotInitializedBalanceException : DbException
    {
        public DbNotInitializedBalanceException(string message) : base(message)
        {
        }
    }
}