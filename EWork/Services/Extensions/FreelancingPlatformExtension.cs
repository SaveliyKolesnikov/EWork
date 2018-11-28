using EWork.Data;
using EWork.Data.Interfaces;
using EWork.Data.Repositories;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.Services.Mappers;
using EWork.Services.Mappers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EWork.Services.Extensions
{
    public static class FreelancingPlatformExtension
    {
        public static IServiceCollection AddEWork(this IServiceCollection service) =>
            service.AddFreelancingPlatformDbContext().AddMappers().AddTagManager().AddRepositories()
                .AddModelManagers().AddMessageMapper().AddUserExtractor().AddPdfReportGenerator()
                .AddScoped<IFreelancingPlatform, EWork>();

        #region Managers

        private static IServiceCollection AddModelManagers(this IServiceCollection service) =>
            service
                .AddNotificationManager().AddJobRecommenderService()
                .AddJobManager().AddProposalManager()
                .AddModeratorManager().AddReviewManager()
                .AddMessageManager().AddBalanceManager();

        private static IServiceCollection AddBalanceManager(this IServiceCollection service) =>
            service.AddScoped<IBalanceManager, BalanceManager>();

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
            service.AddProposalRepository().AddNotificationRepository().AddBalanceRepository()
                .AddJobRepository().AddReviewRepository().AddMessageRepository().AddUserRepository();

        private static IServiceCollection AddUserRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<User>, UserRepository>();

        private static IServiceCollection AddBalanceRepository(this IServiceCollection service) =>
            service.AddScoped<IBalanceRepository, BalanceRepository>();

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

        #region Mappers

        private static IServiceCollection AddMappers(this IServiceCollection service) =>
            service.AddMessageMapper().AddJobMapper();

        private static IServiceCollection AddMessageMapper(this IServiceCollection service) =>
            service.AddSingleton<IMessageMapper, MessageMapper>();

        private static IServiceCollection AddJobMapper(this IServiceCollection service) =>
            service.AddSingleton<IJobMapper, JobMapper>();

        #endregion

        private static IServiceCollection AddPdfReportGenerator(this IServiceCollection service) =>
            service.AddSingleton<IReportGenerator, PdfReportGenerator>();

        private static IServiceCollection AddJobRecommenderService(this IServiceCollection service) =>
            service.AddScoped<IJobRecommender, JobRecommederService>();

        private static IServiceCollection AddUserExtractor(this IServiceCollection service) =>
            service.AddScoped<IUserExtractor, UserExtractor>();

        private static IServiceCollection AddFreelancingPlatformDbContext(this IServiceCollection service) =>
            service.AddScoped<IFreelancingPlatformDbContext, ApplicationDbContext>();
    }
}
