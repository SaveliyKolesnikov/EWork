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
            });

            modelBuilder.Entity<Job>()
                .HasMany(j => j.Offers)
                .WithOne(o => o.Job)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.HiredFreelancer)
                .WithMany(f => f.Jobs)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Freelancer>(freelancerBuilder =>
            {
                freelancerBuilder
                    .HasMany(f => f.Offers)
                    .WithOne(o => o.Sender)
                    .OnDelete(DeleteBehavior.Cascade);

                freelancerBuilder
                    .HasMany(f => f.Jobs)
                    .WithOne(j => j.HiredFreelancer)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Offer>(offerBuilder =>
            {
                // offerBuilder.HasAlternateKey(offer => new {offer.Sender, offer.Job});

                offerBuilder
                    .HasOne(o => o.Job)
                    .WithMany(j => j.Offers)
                    .OnDelete(DeleteBehavior.Restrict);


                offerBuilder
                    .HasOne(o => o.Sender)
                    .WithMany(freelancer => freelancer.Offers)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Tag>()
                .HasAlternateKey(tag => tag.Text);

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
