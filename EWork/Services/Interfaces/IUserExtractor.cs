using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IUserExtractor
    {
        IQueryable<User> GetAll();

        Task<User> FindAsync(Predicate<User> predicate);
    }
}
