﻿using System.Linq;
using EWork.Models;
using Microsoft.EntityFrameworkCore;

namespace EWork.Data.Extensions
{
    public static class DataExtractExtensions
    {
        public static IQueryable<Job> ExtractAll(this DbSet<Job> dbSet)
        {
            return dbSet
                .Include(j => j.Employer)
                    .ThenInclude(e => e.References)
                .Include(j => j.JobTags)
                    .ThenInclude(jt => jt.Tag)
                .Include(j => j.Proposals)
                    .ThenInclude(p => p.Sender)
                        .ThenInclude(f => f.References)
                .Include(j => j.HiredFreelancer);
        }

        public static IQueryable<Proposal> ExtractAll(this DbSet<Proposal> dbSet)
        {
            return dbSet
                .Include(p => p.Sender)
                    .ThenInclude(f => f.References)
                        .ThenInclude(r => r.Sender)
                .Include(p => p.Job)
                    .ThenInclude(j => j.JobTags)
                        .ThenInclude(jt => jt.Tag);
        }

        public static IQueryable<Notification> ExtractAll(this DbSet<Notification> dbSet) =>
            dbSet.Include(n => n.Receiver);
    }
}