using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data;
using Microsoft.AspNetCore.Mvc;
using EWork.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWork.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        public HomeController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult JobBoard()
        {
            var jobs = _db.Jobs.Where(j => j.HiredFreelancer == null)
                .Include(j => j.Employer)
                .ThenInclude(e => e.References)
                .Include(j => j.Offers)
                .Include(j => j.JobTags)
                    .ThenInclude(jt => jt.Tag);
            return View(jobs);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            var deletedJob = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
            if (deletedJob is null)
                return Error();

            _db.Jobs.Remove(deletedJob);
            await _db.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> CreateJob()
        {
            if (!(await _userManager.GetUserAsync(User) is Employeer))
                return NotFound();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJob(Job job, string tags)
        {
            var currentUser = await _userManager.GetUserAsync(User) as Employeer;
            if (currentUser is null)
                return Redirect("JobBoard");

            job.Employer = currentUser;
            job.Offers = new List<Offer>();
            var tagsInDb = _db.Tags;
            var inputTags = tags.Trim().Split(' ');
            var commonTags = tagsInDb.Where(tag => inputTags.Any(t => t == tag.Text));
            var newTagsValues = inputTags.Where(tag => tagsInDb.All(t => tag != t.Text)).Distinct().ToArray();
            var newTags = newTagsValues.Select(tagValue => new Tag {Text = tagValue});
            job.JobTags = new List<JobTags>();
            await _db.Tags.AddRangeAsync(newTags);
            await _db.SaveChangesAsync();
            var newTagsFromBd = _db.Tags.Where(tag => newTagsValues.Contains(tag.Text));
            foreach (var tag in commonTags.Union(newTagsFromBd))
                job.JobTags.Add(new JobTags {Tag = tag});

            await _db.Jobs.AddAsync(job);
            await _db.SaveChangesAsync();
            return Redirect("JobBoard");
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

        [AllowAnonymous]
        [Route("Home/Test")]
        [Route("Test")]
        public async Task<IActionResult> Test()
        {
            var test = new Tests(_db);
            
            return Content((await test.Test1()).ToString());
        }
    }
}
