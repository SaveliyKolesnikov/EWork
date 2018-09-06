using System.Collections.Generic;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Data.Interfaces
{
    public interface IBalanceRepository : IRepository<Balance>
    {
        Task UpdateRangeAsync(IEnumerable<Balance> items);
    }
}