using System;
using EWork.Data;
using EWork.Data.Repositories;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EWork.Services.Extensions
{
    public static class FreelancingPlatformExtension
    {
        public static IServiceCollection AddEWork(this IServiceCollection service)
        {
            var serviceProvider = service.BuildServiceProvider();

            var db = GetAppDbContext(serviceProvider);
            var userManager = GetUserManager(serviceProvider);

            serviceProvider = service.AddModelManagers(db, userManager).BuildServiceProvider();

            var jobManager = serviceProvider.GetService<IJobManager>();
            var proposalManager = serviceProvider.GetService<IProposalManager>();
            var tagManager = serviceProvider.GetService<ITagManager>();
            var notificationManager = serviceProvider.GetService<INotificationManager>();

            return service.AddScoped<IFreelancingPlatform>(provider => new EWork(jobManager, proposalManager, tagManager, notificationManager));
        }

        private static IServiceCollection AddModelManagers(this IServiceCollection service, ApplicationDbContext db = null, UserManager<User> userManager = null)
        {
            if (db is null)
                db = GetAppDbContext(service);

            if (userManager is null)
                userManager = GetUserManager(service);

            return service.AddJobManager(db, userManager)
                .AddProposalManager(db, userManager)
                .AddTagManager(db)
                .AddNotificationManager(userManager);
        }

        private static IServiceCollection AddJobManager(this IServiceCollection service, ApplicationDbContext db = null, UserManager<User> userManager = null)
        {
            if (db is null)
                db = GetAppDbContext(service);

            if (userManager is null)
                userManager = GetUserManager(service);

            var jobRepository = new JobRepository(db, userManager);
            return service.AddScoped<IJobManager>(provider => new JobManager(jobRepository));
        }

        private static IServiceCollection AddTagManager(this IServiceCollection service, ApplicationDbContext db = null)
        {
            if (db is null)
                db = GetAppDbContext(service);

            return service.AddScoped<ITagManager>(provider => new TagManager(db));
        }

        private static IServiceCollection AddProposalManager(this IServiceCollection service, ApplicationDbContext db = null, UserManager<User> userManager = null)
        {
            if (db is null)
                db = GetAppDbContext(service);

            if (userManager is null)
                userManager = GetUserManager(service);

            var proposalRepository = new ProposalRepository(db, userManager);
            return service.AddScoped<IProposalManager>(provider => new ProposalManager(proposalRepository));
        }

        private static IServiceCollection AddNotificationManager(this IServiceCollection service, UserManager<User> userManager = null)
        {
            if (userManager is null)
                userManager = GetUserManager(service);

            return service.AddScoped<INotificationManager>(provider => new NotificationManager(userManager));
        }

        private static ApplicationDbContext GetAppDbContext(IServiceCollection service) =>
            GetAppDbContext(service.BuildServiceProvider());

        private static UserManager<User> GetUserManager(IServiceCollection service) =>
            GetUserManager(service.BuildServiceProvider());

        private static ApplicationDbContext GetAppDbContext(IServiceProvider serviceProvider) =>
            serviceProvider.GetService<ApplicationDbContext>();

        private static UserManager<User> GetUserManager(IServiceProvider serviceProvider) =>
            serviceProvider.GetService<UserManager<User>>();
    }
}
