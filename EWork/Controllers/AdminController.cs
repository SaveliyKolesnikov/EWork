using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EWork.Controllers
{
    [Authorize(Roles = "moderator, administrator")]
    public class AdminController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;

        public AdminController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "administrator")]
        public IActionResult AllJobs(string searchString)
        {
            var jobs = GetJobsByTitle(searchString);
            var adminPageViewModel = new AdminPageViewModel<Job>(jobs, searchString);
            return View(adminPageViewModel);
        }

        [Authorize(Roles = "administrator")]
        public IActionResult Users(string searchString)
        {
            var users = _freelancingPlatform.UserExtractor.GetAll();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                users = users.Where(u => u.UserName.StartsWith(searchString));
            }

            var adminPageViewModel = new AdminPageViewModel<User>(users, searchString);
            return View(adminPageViewModel);
        }

        public IActionResult OpenedDisputes(string searchString)
        {
            var jobs = GetJobsByTitle(searchString).Where(j => j.IsPaymentDenied);
            var adminPageViewModel = new AdminPageViewModel<Job>(jobs, searchString);
            return View(adminPageViewModel);
        }

        protected IQueryable<Job> GetJobsByTitle(string title)
        {
            var jobs = _freelancingPlatform.JobManager.GetAll().Where(j => !j.IsClosed);

            if (!string.IsNullOrWhiteSpace(title))
            {
                jobs = jobs.Where(job => job.Title.StartsWith(title));
            }

            return jobs;
        }
    }
}
