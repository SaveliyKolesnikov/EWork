using System;
using System.Collections.Generic;
using System.Text;
using EWork.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EWork.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Employeer> Employeers { get; set; }
        public DbSet<Freelancer> Freelancers { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Offer> Offers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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
    }
}
