using System.Collections.Generic;
using System.Linq;
using EWork.Models;

namespace EWork.ViewModels
{
    public class JobBoardViewModel
    {
        public JobBoardViewModel(IQueryable<Job> jobs, IEnumerable<string> usedTags, string searchUrl, string getJobsAjaxMethodUrl, User currentUser = null)
        {
            Jobs = jobs;
            UsedTags = usedTags;
            SearchUrl = searchUrl;
            GetJobsAjaxMethodUrl = getJobsAjaxMethodUrl;
            CurrentUser = currentUser;
        }

        public User CurrentUser { get; }
        public IQueryable<Job> Jobs { get; }
        public IEnumerable<string> UsedTags { get; }
        public string SearchUrl { get; }
        public string GetJobsAjaxMethodUrl { get; }
    }
}
