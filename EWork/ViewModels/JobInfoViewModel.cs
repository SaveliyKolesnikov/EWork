using EWork.Models;

namespace EWork.ViewModels
{
    public class JobInfoViewModel
    {
        public JobInfoViewModel(User currentUser, Job job, Proposal proposal)
        {
            Job = job;
            Proposal = proposal;
            CurrentUser = currentUser;
        }

        public Job Job { get; }
        public User CurrentUser { get; }
        public Proposal Proposal { get; }
    }
}