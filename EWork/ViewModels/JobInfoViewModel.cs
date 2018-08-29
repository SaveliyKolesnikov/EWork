using EWork.Models;

namespace EWork.ViewModels
{
    public class JobInfoViewModel
    {
        public JobInfoViewModel(User currentUser, Job job, Proposal proposal, string pathToProfilePhotos)
        {
            Job = job;
            Proposal = proposal;
            PathToProfilePhotos = pathToProfilePhotos;
            CurrentUser = currentUser;
        }
        public Job Job { get; }
        public User CurrentUser { get; }
        public Proposal Proposal { get; }
        public string PathToProfilePhotos { get; }
    }
}