using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data;
using Microsoft.AspNetCore.Mvc;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWork.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;

        public HomeController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult JobBoard()
        {
            var jobs = _freelancingPlatform.JobManager.GetAll();
            return View(jobs);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            var deletedJob = await _freelancingPlatform.JobManager.FindAsync(job => job.Id == jobId);
            if (deletedJob is null)
                return BadRequest();

            await _freelancingPlatform.JobManager.DeleteAsync(deletedJob);
            return Ok();
        }

        public IActionResult CreateJob()
        {
            if (!User.IsInRole("employer"))
                return NotFound();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJob(Job job, string tags)
        {
            if (!(await _userManager.GetUserAsync(User) is Employer currentUser))
                return Redirect("JobBoard");

            job.Employer = currentUser;
            job.Proposals = new List<Proposal>();
            job.JobTags = new List<JobTags>();

            var newTags = await _freelancingPlatform.TagManager.AddTagsRangeAsync(tags.Trim().Split(' '));
            foreach (var tag in newTags)
                job.JobTags.Add(new JobTags { Tag = tag });

            await _freelancingPlatform.JobManager.AddAsync(job);
            return Redirect("JobBoard");
        }

        [HttpGet]
        public async Task<IActionResult> Job(int id)
        {
            var job = await _freelancingPlatform.JobManager.FindAsync(j => j.Id == id);

            return job is null ? Error() : View(job);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[AllowAnonymous]
        //[Route("Home/Test")]
        //[Route("Test")]
        //public async Task<IActionResult> Test()
        //{
        //    var test = new Tests(_db);

        //    return Content((await test.Test1()).ToString());
        //}
    }
}
