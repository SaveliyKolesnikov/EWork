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
            service.AddFreelancingPlatformDbContext()
                .AddRepositories().AddModelManagers().AddMessageMapper()
                .AddScoped<IFreelancingPlatform, EWork>();

        #region Managers

        private static IServiceCollection AddModelManagers(this IServiceCollection service) =>
            service
                .AddJobManager().AddProposalManager()
                .AddModeratorManager().AddTagManager()
                .AddNotificationManager().AddReviewManager()
                .AddMessageManager();

        private static IServiceCollection AddMessageManager(this IServiceCollection service) =>
            service.AddScoped<IMessageManager, MessageManager>();

        private static IServiceCollection AddJobManager(this IServiceCollection service) =>
            service.AddScoped<IJobManager, JobManager>();

        private static IServiceCollection AddReviewManager(this IServiceCollection service) =>
            service.AddScoped<IReviewManager, ReviewManager>();

        private static IServiceCollection AddTagManager(this IServiceCollection service) =>
            service.AddScoped<ITagManager, TagManager>();

        private static IServiceCollection AddProposalManager(this IServiceCollection service) =>
            service.AddScoped<IProposalManager, ProposalManager>();

        private static IServiceCollection AddNotificationManager(this IServiceCollection service) =>
            service.AddScoped<INotificationManager, NotificationManager>();

        private static IServiceCollection AddModeratorManager(this IServiceCollection service) =>
            service.AddScoped<IModeratorManager, ModeratorManager>();

        #endregion

        #region Repositories

        private static IServiceCollection AddRepositories(this IServiceCollection service) =>
            service.AddProposalRepository().AddNotificationRepository()
                .AddJobRepository().AddReviewRepository().AddMessageRepository();

        private static IServiceCollection AddProposalRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<Proposal>, ProposalRepository>();

        private static IServiceCollection AddMessageRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<Message>, MessageRepository>();

        private static IServiceCollection AddNotificationRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<Notification>, NotificationRepository>();

        private static IServiceCollection AddJobRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<Job>, JobRepository>();

        private static IServiceCollection AddReviewRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<Review>, ReviewRepository>();
        #endregion

        private static IServiceCollection AddMessageMapper(this IServiceCollection service) =>
            service.AddSingleton<IMessageMapper, MessageMapper>();

        private static IServiceCollection AddRandomStringGenerator(this IServiceCollection service) =>
            service.AddSingleton<IRandomStringGenerator, RandomStringGenerator>();

        private static IServiceCollection AddFreelancingPlatformDbContext(this IServiceCollection service) =>
            service.AddScoped<IFreelancingPlatiformDbContext, ApplicationDbContext>();
    }
}
