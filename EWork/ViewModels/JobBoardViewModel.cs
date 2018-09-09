using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.ViewModels
{
    public class JobBoardViewModel
    {
        public JobBoardViewModel(IQueryable<Job> jobs, IEnumerable<string> usedTags)
        {
            Jobs = jobs;
            UsedTags = usedTags;
        }

        public IQueryable<Job> Jobs { get; }
        public IEnumerable<string> UsedTags { get; }
    }
}
