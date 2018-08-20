using System;
using System.Threading;
using System.Threading.Tasks;
using EWork.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EWork.Services.Interfaces
{
    public interface IFreelancingPlatiformDbContext : IDisposable
    {
        DbSet<Job> Jobs { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Proposal> Proposals { get; set; }
        DbSet<Employer> Employeers { get; set; }
        DbSet<Freelancer> Freelancers { get; set; }

        Task SaveChangesAsync();
        EntityEntry<T> Entry<T>(T obj) where T : class;
    }
}