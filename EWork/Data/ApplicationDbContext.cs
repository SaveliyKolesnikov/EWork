using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EWork.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>, IFreelancingPlatformDbContext
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
        public DbSet<Freelancer> Freelancers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<FreelancerTags> FreelancerTags { get; set; }

        async Task IFreelancingPlatformDbContext.SaveChangesAsync() => await SaveChangesAsync();

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CreateJobTagsModel();
            CreateFreelancerTagsModel();

            modelBuilder.Entity<User>(userBuilder =>
            {
                userBuilder.HasIndex(u => u.UserName).IsUnique();
                userBuilder.HasIndex(u => u.Email).IsUnique();
                userBuilder.HasOne(u => u.Balance)
                    .WithOne(b => b.User)
                    .OnDelete(DeleteBehavior.Cascade);

                userBuilder.HasMany(u => u.Reviews)
                    .WithOne(r => r.User)
                    .OnDelete(DeleteBehavior.Restrict);

                userBuilder.HasMany(u => u.SentReviews)
                    .WithOne(r => r.Sender);

                userBuilder.HasMany(u => u.SentMessages)
                    .WithOne(r => r.Sender)
                    .OnDelete(DeleteBehavior.Restrict);

                userBuilder.HasMany(u => u.ReceivedMessages)
                    .WithOne(r => r.Receiver)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Freelancer>()
                .HasMany(f => f.Proposals)
                .WithOne(p => p.Sender)
                .OnDelete(DeleteBehavior.Restrict); ;


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
                    .WithMany()
                    .HasForeignKey(jt => jt.TagId);
            }

            void CreateFreelancerTagsModel()
            {
                modelBuilder.Entity<FreelancerTags>()
                    .HasKey(o => new { o.FreelancerId, o.TagId });

                modelBuilder.Entity<FreelancerTags>()
                    .HasOne(jt => jt.Freelancer)
                    .WithMany(j => j.Tags)
                    .HasForeignKey(jt => jt.FreelancerId);

                modelBuilder.Entity<FreelancerTags>()
                    .HasOne(jt => jt.Tag)
                    .WithMany()
                    .HasForeignKey(jt => jt.TagId);
            }
        }

        public DbSet<EWork.Models.Notification> Notification { get; set; }

    }
}
