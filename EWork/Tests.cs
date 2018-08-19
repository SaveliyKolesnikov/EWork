using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data;
using EWork.Models;
using Microsoft.EntityFrameworkCore;

namespace EWork
{
    public class Tests
    {
        private readonly ApplicationDbContext _db;
        public Tests(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Test1()
        {
            await TruncateAllTables(_db);
            var employeer1 = new Employeer
            {
                UserName = "Saveliy.K",
                PasswordHash = "qwerty123",
                Email = "s.koles33@gmail.com",
                Name = "Saveliy",
                Surname = "Kolesnikov",
                References = new List<Reference>(),
                Balance = new Balance(),
                Jobs = new List<Job>()
            };

            var userJob = new Job
            {
                Title = "Test job",
                Description = "Just test job",
                Budget = 5000.99m,
                Deadline = DateTime.Now.AddDays(5),
                JobTags = new List<JobTags>()
            };

            var tagsInDb = _db.Tags;
            var tags = new[]
            {
                new Tag {Text = "Job"},
                new Tag {Text = "Savely"},
                new Tag {Text = "Ensure payment"}
            };

            var commonTags = tagsInDb.Where(tag => tags.Any(t => t.Text == tag.Text));
            var newTags = tags.Where(tag => tagsInDb.All(t => tag.Text != t.Text)).Distinct().ToArray();
            userJob.JobTags = new List<JobTags>();
            await _db.Tags.AddRangeAsync(newTags);
            await _db.SaveChangesAsync();

            var newTagsVal = newTags.Select(tag => tag.Text);
            var newTagsFromBd = _db.Tags.Where(tag => newTagsVal.Contains(tag.Text));
            foreach (var tag in commonTags.Union(newTagsFromBd))
                userJob.JobTags.Add(new JobTags { Tag = tag });

            employeer1.Jobs.Add(userJob);

            var freelancer1 = new Freelancer
            {
                UserName = "vadim.m",
                PasswordHash = "qwerty321",
                Email = "vadim.maturin@gmail.com",
                Name = "Vadim",
                Surname = "Maturin",
                References = new List<Reference>(),
                Balance = new Balance(),
                Jobs = new List<Job>(),
                Offers = new List<Offer>()
            };

            freelancer1.Offers.Add(new Offer
            {
                Job = userJob,
                Text = "All will be good"
            });

            var freelancer2 = new Freelancer
            {
                UserName = "Armosty",
                PasswordHash = "Palopalo",
                Email = "dmitry.striga@nure.ua",
                Name = "Dimon",
                Surname = "Striga",
                References = new List<Reference>() { new Reference() { Sender = employeer1, Text = "goog", Value = 9 } },
                Balance = new Balance(),
                Jobs = new List<Job>() { userJob },
                Offers = new List<Offer>() //{ new Offer() {Job = userJob, Text = "I'm Dimon." } }
            };

            await _db.Employeers.AddAsync(employeer1);
            await _db.Freelancers.AddAsync(freelancer1);
            await _db.Freelancers.AddAsync(freelancer2);
            await _db.SaveChangesAsync();

            await TruncateAllTables(_db);
            return true;
        }

        private async Task TruncateAllTables(ApplicationDbContext context)
        {
            var employeers = context.Employeers
                .Include(e => e.Jobs)
                .ThenInclude(j => j.Offers)
                .Include(e => e.References);

            foreach (var employeer in employeers)
            {
                foreach (var employeerJob in employeer.Jobs)
                {
                    _db.Offers
                        .RemoveRange(employeerJob.Offers);
                }
            }

            context.Employeers.RemoveRange(employeers);
            var freelancers = context.Freelancers.Include(f => f.Offers).Include(f => f.References);
            _db.Offers
                .RemoveRange(freelancers.SelectMany(f => f.Offers, (freelancer, offer) => offer));
            context.Freelancers.RemoveRange(freelancers);
            await context.SaveChangesAsync();
        }
    }
}
