using EWork.Models;

namespace EWork.ViewModels
{
    public class JobInfoViewModel
    {
        public JobInfoViewModel(Job job, Proposal proposal)
        {
            Job = job;
            Proposal = proposal;
        }

        public Job Job { get; }
        public Proposal Proposal { get; }
    }
}