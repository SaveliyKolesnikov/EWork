using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EWork.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>, IFreelancingPlatiformDbContext
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
        public DbSet<Freelancer> Freelancers { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        async Task IFreelancingPlatiformDbContext.SaveChangesAsync() => await SaveChangesAsync();

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreateJobTagsModel();

            modelBuilder.Entity<User>(userBuilder =>
            {
                userBuilder.HasIndex(u => u.UserName).IsUnique();
                userBuilder.HasIndex(u => u.Email).IsUnique();
                userBuilder.HasOne(u => u.Balance)
                    .WithOne(b => b.User)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);

            void CreateJobTagsModel()
            {
                modelBuilder.Entity<JobTags>()
                    .HasKey(o => new { o.JobId, o.TagId });

                modelBuilder.Entity<JobTags>()
                    .HasOne(jt => jt.Job)
                    .WithMany(j => j.JobTags)
                    .HasForeignKey(jt => jt.JobId);

                modelBuilder.Entity<JobTags>()
                    .HasOne(jt => jt.Tag)
                    .WithMany(o => o.JobTags)
                    .HasForeignKey(jt => jt.TagId);
            }
        }

        public DbSet<EWork.Models.Notification> Notification { get; set; }

    }
}
