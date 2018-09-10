using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.ViewModels
{
    public class JobBoardViewModel
    {
        public JobBoardViewModel(IQueryable<Job> jobs, IEnumerable<string> usedTags, string searchUrl, int takeAmount = 5)
        {
            Jobs = jobs;
            UsedTags = usedTags;
            SearchUrl = searchUrl;
            TakeAmount = takeAmount;
        }

        public IQueryable<Job> Jobs { get; }
        public IEnumerable<string> UsedTags { get; }
        public int TakeAmount { get; }
        public string SearchUrl { get; }
    }
}
