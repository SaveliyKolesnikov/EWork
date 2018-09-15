using System;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class UserExtractor : IUserExtractor
    {
        private readonly IRepository<User> _repository;

        public UserExtractor(IRepository<User> repository) => _repository = repository;

        public IQueryable<User> GetAll() => _repository.GetAll();

        public Task<User> FindAsync(Predicate<User> predicate) => _repository.FindAsync(predicate);
    }
}
