using System.Linq;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Exceptions;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.Services.Mappers.Interfaces;
using EWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EWork.Controllers
{
    [Authorize(Roles = "moderator, administrator")]
    public class AdminController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<FreelancingPlatformConfig> _freelancingPlatformOptions;
        private readonly IJobMapper _jobMapper;
        private readonly int _takeAmount;

        public AdminController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager,
            IOptions<FreelancingPlatformConfig> freelancingPlatformOptions, IJobMapper jobMapper)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
            _freelancingPlatformOptions = freelancingPlatformOptions;
            _jobMapper = jobMapper;
            _takeAmount = freelancingPlatformOptions.Value.TakeAmount;
        }

        public IActionResult Index() => View();

        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> AllJobs(string searchString)
        {
            var jobs = await GetJobsByTitle(searchString).Take(_takeAmount).ToArrayAsync();
            var adminPageViewModel = new AdminPageViewModel<Job>(jobs, _takeAmount, searchString);
            return View(adminPageViewModel);
        }

        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Users(string searchString)
        {
            var users = GetUsersByUserName(searchString).Take(_takeAmount);

            foreach (var user in users)
            {
                user.Jobs = await GetUserJobs(user.Id).ToListAsync();
            }

            var adminPageViewModel = new AdminPageViewModel<User>(await users.ToArrayAsync(), _takeAmount, searchString);
            return View(adminPageViewModel);
        }

        private IQueryable<Job> GetUserJobs(string userId) =>
            _freelancingPlatform.JobManager.GetAll().Where(j =>
                !(j.IsClosed && j.IsPaymentDenied) && j.Employer.Id == userId || j.HiredFreelancer.Id == userId);

        private IQueryable<User> GetUsersByUserName(string searchString)
        {
            var users = _freelancingPlatform.UserExtractor.GetAll();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                users = users.Where(u => u.UserName.Contains(searchString));
            }

            return users;
        }

        public async Task<IActionResult> OpenedDisputes(string searchString)
        {
            var jobs = await GetJobsByTitle(searchString)
                .Where(j => j.IsPaymentDenied)
                .Take(_takeAmount)
                .ToArrayAsync();

            var adminPageViewModel = new AdminPageViewModel<Job>(jobs, _takeAmount, searchString);
            return View(adminPageViewModel);
        }

        private IQueryable<Job> GetJobsByTitle(string title)
        {
            var jobs = _freelancingPlatform.JobManager.GetAll().Where(j => !j.IsClosed);

            if (!string.IsNullOrWhiteSpace(title))
            {
                jobs = jobs.Where(job => job.Title.StartsWith(title));
            }

            return jobs;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
#pragma warning disable 1998
        public async Task<IActionResult> DeleteUser(string userId)
#pragma warning restore 1998
        {
#if AllowUserDeletion
            var deletedUser = await _userManager.FindByIdAsync(userId);
            if (deletedUser is null || deletedUser is Administrator)
                return UnprocessableEntity(userId);

            await _userManager.DeleteAsync(deletedUser);
#endif

            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(string userId) =>
            await ChangeBanStatus(userId, true);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser(string userId) =>
            await ChangeBanStatus(userId, false);

        private async Task<IActionResult> ChangeBanStatus(string userId, bool banStatus)
        {
            var blockedUser = await _userManager.FindByIdAsync(userId);
            if (blockedUser is null || blockedUser is Administrator)
                return UnprocessableEntity(userId);

            if (blockedUser.IsBlocked != banStatus)
            {
                blockedUser.IsBlocked = banStatus;
                await _userManager.UpdateAsync(blockedUser);
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplenishBalance(int balanceId, decimal amount) =>
            await DoTransferAsync(_freelancingPlatformOptions.Value.BalanceId, balanceId, amount);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DecreaseBalance(int balanceId, decimal amount) =>
            await DoTransferAsync(balanceId, _freelancingPlatformOptions.Value.BalanceId, amount);

        private async Task<IActionResult> DoTransferAsync(int senderBalanceId, int recipientBalanceId, decimal amount)
        {
            if (amount < 0)
                return UnprocessableEntity(amount);

            if (amount == 0)
                return RedirectToAction("Users");

            var senderBalance = await _freelancingPlatform.BalanceManager.FindAsync(b => b.Id == senderBalanceId);
            if (senderBalance is null)
                return UnprocessableEntity(recipientBalanceId);

            var recipientBalance = await _freelancingPlatform.BalanceManager.FindAsync(b => b.Id == recipientBalanceId);
            if (recipientBalance is null)
                return UnprocessableEntity(recipientBalanceId);

            try
            {
                await _freelancingPlatform.BalanceManager.TransferMoneyAsync(senderBalance: senderBalance,
                    recipientBalance: recipientBalance, amount: amount);
            }
            catch (NotEnoughMoneyException e)
            {
                return Content(e.Message);
            }

            return RedirectToAction("Users");
        }

        #region AjaxMethods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetUsersAjax(int skipAmount, int takeAmount, string searchString)
        {
            var users = await GetUsersByUserName(searchString).Skip(skipAmount).Take(takeAmount).ToArrayAsync();
            var res =  users.Select(u => new
            {
                u.Id,
                u.Role,
                u.UserName,
                u.FullName,
                u.SignUpDate,
                u.Balance.Money,
                u.IsBlocked,
                Rating = u.Reviews.Count == 0 ? 0 : u.Reviews.Average(r => r.Value),
                Jobs = GetUserJobs(u.Id).Select(j => j.Id).ToList(),
                BalanceId = u.Balance.Id
            });


            return Json(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetJobsAjax(int skipAmount, int takeAmount, string searchString)
        {
            var jobs = await GetJobsByTitle(searchString).Skip(skipAmount).Take(takeAmount).ToArrayAsync();
            var res = _jobMapper.MapRange(jobs);

            return Json(res);
        }

        public async Task<JsonResult> GetDisputedJobsAjax(int skipAmount, int takeAmount, string searchString)
        {
            var jobs = await GetJobsByTitle(searchString)
                .Where(j => j.IsPaymentDenied)
                .Skip(skipAmount)
                .Take(takeAmount)
                .ToArrayAsync();
            var res = _jobMapper.MapRange(jobs);

            return Json(res);
        }

        #endregion
    }
}
