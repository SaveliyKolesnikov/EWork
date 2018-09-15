using System.Collections.Generic;
using System.Linq;
using EWork.Models;

namespace EWork.ViewModels
{
    public class JobBoardViewModel
    {
        public JobBoardViewModel(IQueryable<Job> jobs, IEnumerable<string> usedTags, string searchUrl, string getJobsAjaxMethodUrl, User currentUser = null)
            => (Jobs, UsedTags, SearchUrl, GetJobsAjaxMethodUrl, CurrentUser) = (jobs, usedTags, searchUrl, getJobsAjaxMethodUrl, currentUser);

        public User CurrentUser { get; }
        public IQueryable<Job> Jobs { get; }
        public IEnumerable<string> UsedTags { get; }
        public string SearchUrl { get; }
        public string GetJobsAjaxMethodUrl { get; }
    }
}
