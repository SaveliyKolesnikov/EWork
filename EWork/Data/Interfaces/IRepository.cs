using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWork.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T item);
        Task DeleteAsync(T item);
        Task DeleteRangeAsync(IEnumerable<T> items);
        Task UpdateAsync(T item);
        Task<T> FindAsync(Predicate<T> predicate);
        IQueryable<T> GetAll();
    }
}