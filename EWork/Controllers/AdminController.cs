using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWork.Controllers
{
    [Authorize(Roles = "moderator, administrator")]
    public class AdminController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;

        public AdminController(IFreelancingPlatform freelancingPlatform)
        {
            _freelancingPlatform = freelancingPlatform;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AllJobs(string searchString)
        {
            return View(_freelancingPlatform.JobManager.GetAll().Where(j => !j.IsClosed));
        }

        public async Task<IActionResult> OpenedDisputes(string searchString)
        {
            return View(_freelancingPlatform.JobManager.GetAll().Where(j => !j.IsClosed && j.IsPaymentDenied));
        }
    }
}
