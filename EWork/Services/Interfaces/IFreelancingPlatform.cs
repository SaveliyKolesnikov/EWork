namespace EWork.Services.Interfaces
{
    public interface IFreelancingPlatform
    {
        ITagManager TagManager { get; }
        IJobManager JobManager { get; }
        IReviewManager ReviewManager { get; }
        IProposalManager ProposalManager { get; }
        INotificationManager NotificationManager { get; }
    }
}