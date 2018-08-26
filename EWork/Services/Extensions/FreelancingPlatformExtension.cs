using EWork.Data;
using EWork.Data.Interfaces;
using EWork.Data.Repositories;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EWork.Services.Extensions
{
    public static class FreelancingPlatformExtension
    {
        public static IServiceCollection AddEWork(this IServiceCollection service) =>
            service.AddFreelancingPlatformDbContext().AddRepositories().AddModelManagers()
                .AddScoped<IFreelancingPlatform, EWork>();

        #region Managers

        private static IServiceCollection AddModelManagers(this IServiceCollection service) =>
            service.AddJobManager().AddProposalManager().AddTagManager().AddNotificationManager();

        private static IServiceCollection AddJobManager(this IServiceCollection service) =>
            service.AddScoped<IJobManager, JobManager>();

        private static IServiceCollection AddTagManager(this IServiceCollection service) =>
            service.AddScoped<ITagManager, TagManager>();

        private static IServiceCollection AddProposalManager(this IServiceCollection service) =>
            service.AddScoped<IProposalManager, ProposalManager>();

        private static IServiceCollection AddNotificationManager(this IServiceCollection service) =>
            service.AddScoped<INotificationManager, NotificationManager>();

        #endregion

        #region Repositories

        private static IServiceCollection AddRepositories(this IServiceCollection service) =>
            service.AddProposalRepository().AddNotificationRepository().AddJobRepository();

        private static IServiceCollection AddProposalRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<Proposal>, ProposalRepository>();

        private static IServiceCollection AddNotificationRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<Notification>, NotificationRepository>();

        private static IServiceCollection AddJobRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<Job>, JobRepository>();

        #endregion

        private static IServiceCollection AddFreelancingPlatformDbContext(this IServiceCollection service) =>
            service.AddScoped<IFreelancingPlatiformDbContext, ApplicationDbContext>();
    }
}
