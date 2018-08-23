using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using EWork.Data;
using EWork.Data.Extensions;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EWork.Services
{
    public class EWork : IFreelancingPlatform
    {
        public ITagManager TagManager { get; }
        public IJobManager JobManager { get; }
        public IProposalManager ProposalManager { get; }
        public INotificationManager NotificationManager { get; }

        public EWork(IJobManager jobManager, IProposalManager proposalManager, ITagManager tagManager, INotificationManager notificationManager)
        {
            JobManager = jobManager;
            ProposalManager = proposalManager;
            TagManager = tagManager;
            NotificationManager = notificationManager;
        }
    }
}
