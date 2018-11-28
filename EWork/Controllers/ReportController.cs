using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EWork.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IJobManager _jobManager;
        private readonly IReportGenerator _reportGenerator;
        private readonly UserManager<User> _userManager;

        public ReportController(IJobManager jobManager, IReportGenerator reportGenerator, UserManager<User> userManager)
        {
            _jobManager = jobManager;
            _reportGenerator = reportGenerator;
            _userManager = userManager;
        }

        [Authorize(Roles = "freelancer, employer")]
        public async Task<FileResult> GetContractsReport()
        {
            var user = await _userManager.GetUserAsync(User);
            var userContracts = Enumerable.Empty<Job>().AsQueryable();
            switch (user)
            {
                case Employer _:
                    userContracts = GetActiveJobs().Where(j => j.Employer.Id == user.Id);
                    break;
                case Freelancer _:
                    userContracts = GetActiveJobs().Where(j => j.HiredFreelancer.Id == user.Id);
                    break;
            }
            var report = _reportGenerator.JobsReport(user, userContracts);
            const string fileType = "application/pdf";
            var fileName = $"{user.UserName} contracts {DateTime.Now:u}.pdf";
            return File(report, fileType, fileName);
        }

        private IQueryable<Job> GetActiveJobs() =>
            _jobManager.GetAll().Where(j => !j.IsClosed && j.HiredFreelancer != null);
    }
}