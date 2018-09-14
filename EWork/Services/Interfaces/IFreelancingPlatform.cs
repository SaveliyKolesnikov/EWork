namespace EWork.Services.Interfaces
{
    public interface IFreelancingPlatform
    {
        ITagManager TagManager { get; }
        IJobManager JobManager { get; }
        IUserExtractor UserExtractor { get; }
        IReviewManager ReviewManager { get; }
        IBalanceManager BalanceManager { get; }
        IProposalManager ProposalManager { get; }
        INotificationManager NotificationManager { get; }
    }
}