using System.Collections.Generic;
using System.Linq;
using EWork.Controllers;
using EWork.Models;

namespace EWork.ViewModels
{
    public class JobBoardViewModel
    {
        public JobBoardViewModel(IQueryable<Job> jobs, JobController.FilterModel filterModel, string searchUrl, string getJobsAjaxMethodUrl, int takeAmount, User currentUser = null)
        => (Jobs, FilterModel, SearchUrl, GetJobsAjaxMethodUrl, TakeAmount, CurrentUser) =
           (jobs, filterModel, searchUrl, getJobsAjaxMethodUrl, takeAmount, currentUser);

        public User CurrentUser { get; }
        public IQueryable<Job> Jobs { get; }
        public JobController.FilterModel FilterModel { get; }
        public string SearchUrl { get; }
        public string GetJobsAjaxMethodUrl { get; }
        public int TakeAmount { get; }
    }
}
