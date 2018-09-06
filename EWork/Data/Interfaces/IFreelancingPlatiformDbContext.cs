using System;
using System.Threading.Tasks;
using EWork.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EWork.Data.Interfaces
{
    public interface IFreelancingPlatiformDbContext : IDisposable
    {
        DbSet<Job> Jobs { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Review> Reviews { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<Balance> Balances { get; set; }
        DbSet<Proposal> Proposals { get; set; }
        DbSet<Employer> Employers { get; set; }
        DbSet<Moderator> Moderators { get; set; }
        DbSet<Freelancer> Freelancers { get; set; }
        DbSet<Notification> Notifications { get; set; }

        Task SaveChangesAsync();
        EntityEntry<T> Entry<T>(T obj) where T : class;
    }
}