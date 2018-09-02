using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class MessageManager : IMessageManager
    {
        private readonly IRepository<Message> _repository;

        public MessageManager(IRepository<Message> repository) => _repository = repository;

        public Task AddAsync(Message item) => _repository.AddAsync(item);

        public Task DeleteAsync(Message item) => _repository.DeleteAsync(item);

        public Task DeleteRangeAsync(IEnumerable<Message> items) => _repository.DeleteRangeAsync(items);

        public Task<Message> FindAsync(Predicate<Message> predicate) => _repository.FindAsync(predicate);

        public IQueryable<Message> GetAll() => _repository.GetAll();

        public Task UpdateAsync(Message item) => _repository.UpdateAsync(item);
    }
}