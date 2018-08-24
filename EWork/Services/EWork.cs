using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class EWork : IFreelancingPlatform
    {
        public ITagManager TagManager { get; }
        public IJobManager JobManager { get; }
        public IProposalManager ProposalManager { get; }
        public INotificationManager NotificationManager { get; }

        public EWork(IJobManager jobManager, IProposalManager proposalManager, ITagManager tagManager, INotificationManager notificationManager)
        {
            JobManager = jobManager;
            ProposalManager = proposalManager;
            TagManager = tagManager;
            NotificationManager = notificationManager;
        }
    }
}
