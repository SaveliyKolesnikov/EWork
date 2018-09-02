using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Extensions;
using EWork.Data.Interfaces;
using EWork.Models;
using Microsoft.EntityFrameworkCore;

namespace EWork.Data.Repositories
{
    public class MessageRepository : IRepository<Message>
    {
        private readonly IFreelancingPlatiformDbContext _db;

        public MessageRepository(IFreelancingPlatiformDbContext db) => _db = db;

        public async Task AddAsync(Message message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (message.Sender is null)
                throw new ArgumentNullException(nameof(message.Sender));

            if (message.Receiver is null)
                throw new ArgumentNullException(nameof(message.Receiver));

            if (message.Text is null)
                throw new ArgumentNullException(nameof(message.Text));

            try
            {
                AttachUserToDbContext(message.Receiver);
                AttachUserToDbContext(message.Sender);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();

            void AttachUserToDbContext(User user)
            {
                switch (user)
                {
                    case null:
                        throw new ArgumentNullException(nameof(user));
                    case Employer employer:
                        _db.Employers.Attach(employer);
                        break;
                    case Freelancer freelancer:
                        _db.Freelancers.Attach(freelancer);
                        break;
                    case Moderator moderator:
                        _db.Moderators.Attach(moderator);
                        break;
                }
            }
        }

        public async Task DeleteAsync(Message message)
        {
            if (message is null)
                return;

            _db.Messages.Remove(message);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Message> items)
        {
            if (items is null)
                return;

            _db.Messages.RemoveRange(items);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Message message)
        {
            _db.Entry(message).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<Message> FindAsync(Predicate<Message> predicate) =>
            await GetAll().FirstOrDefaultAsync(message => predicate(message));

        public IQueryable<Message> GetAll() => _db.Messages.ExtractAll();
    }
}
