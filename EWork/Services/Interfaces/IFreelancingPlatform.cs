namespace EWork.Services.Interfaces
{
    public interface IFreelancingPlatform
    {
        ITagManager TagManager { get; }
        IJobManager JobManager { get; }
        IProposalManager ProposalManager { get; }
        INotificationManager NotificationManager { get; }
    }
}