using System.Collections.Generic;
using System.Linq;
using EWork.Models;

namespace EWork.ViewModels
{
    public class JobBoardViewModel
    {
        public JobBoardViewModel(IQueryable<Job> jobs, IEnumerable<string> usedTags, string searchUrl, string getJobsAjaxMethodUrl, int takeAmount = 5)
        {
            Jobs = jobs;
            UsedTags = usedTags;
            SearchUrl = searchUrl;
            GetJobsAjaxMethodUrl = getJobsAjaxMethodUrl;
            TakeAmount = takeAmount;
        }

        public IQueryable<Job> Jobs { get; }
        public IEnumerable<string> UsedTags { get; }
        public int TakeAmount { get; }
        public string SearchUrl { get; }
        public string GetJobsAjaxMethodUrl { get; }
    }
}
