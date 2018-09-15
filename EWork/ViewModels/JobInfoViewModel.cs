using EWork.Models;

namespace EWork.ViewModels
{
    public class JobInfoViewModel
    {
        public JobInfoViewModel(User currentUser, Job job, Proposal proposal, string pathToProfilePhotos)
            => (CurrentUser, Job, Proposal, PathToProfilePhotos) = (currentUser, job, proposal, pathToProfilePhotos);

        public Job Job { get; }
        public User CurrentUser { get; }
        public Proposal Proposal { get; }
        public string PathToProfilePhotos { get; }
    }
}