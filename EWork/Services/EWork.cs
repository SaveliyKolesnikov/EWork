using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class EWork : IFreelancingPlatform
    {
        public ITagManager TagManager { get; }
        public IJobManager JobManager { get; }
        public IUserExtractor UserExtractor { get; }
        public IReviewManager ReviewManager { get; }
        public IBalanceManager BalanceManager { get; }
        public IProposalManager ProposalManager { get; }
        public INotificationManager NotificationManager { get; }

        public EWork(ITagManager tagManager,
            IJobManager jobManager, 
            IUserExtractor userExtractor,
            IReviewManager reviewManager, 
            IBalanceManager balanceManager,
            IProposalManager proposalManager,
            INotificationManager notificationManager
            )
        {
            TagManager = tagManager;
            JobManager = jobManager;
            UserExtractor = userExtractor;
            ReviewManager = reviewManager;
            BalanceManager = balanceManager;
            ProposalManager = proposalManager;
            NotificationManager = notificationManager;
        }
    }
}
