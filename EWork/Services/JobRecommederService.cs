using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EWork.Services
{
    public class JobRecommederService : IJobRecommender
    {
        private readonly IFreelancingPlatformDbContext _dbContext;
        private readonly INotificationManager _notificationManager;

        public JobRecommederService(IFreelancingPlatformDbContext dbContext, INotificationManager notificationManager)
        {
            _dbContext = dbContext;
            _notificationManager = notificationManager;
        }

        public async Task RecommendAsync(Job job)
        {
            if (job.JobTags is null || job.JobTags.Count is 0)
                return;

            var jobTags = job.JobTags.Select(jt => jt.Tag);

            var matchedFreelancers = await _dbContext.FreelancerTags
                                    .Where(ft => jobTags.Any(tag => tag.Id == ft.TagId))
                                    .Include(ft => ft.Freelancer).Distinct()
                                    .Select(ft => ft.Freelancer).ToArrayAsync();

            foreach (var matchedFreelancer in matchedFreelancers)
            {
                var notification = new Notification
                {
                    CreatedDate = DateTime.UtcNow,
                    Receiver = matchedFreelancer,
                    Source = $"/Job/JobInfo?jobid={job.Id}",
                    Title = "Here is the project matching your profile and skills"
                };
                await _notificationManager.AddNotificationAsync(notification);
            }
        }
    }
}