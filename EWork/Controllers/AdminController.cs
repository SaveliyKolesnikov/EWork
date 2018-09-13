﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Exceptions;
using EWork.Models;
using EWork.Services.Interfaces;
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

        public AdminController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager,
            IOptions<FreelancingPlatformConfig> freelancingPlatformOptions)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
            _freelancingPlatformOptions = freelancingPlatformOptions;
        }

        public IActionResult Index() => View();

        [Authorize(Roles = "administrator")]
        public IActionResult AllJobs(string searchString)
        {
            var jobs = GetJobsByTitle(searchString);
            var adminPageViewModel = new AdminPageViewModel<Job>(jobs, searchString);
            return View(adminPageViewModel);
        }

        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Users(string searchString)
        {
            var users = _freelancingPlatform.UserExtractor.GetAll();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                users = users.Where(u => u.UserName.StartsWith(searchString));
            }

            foreach (var user in users)
            {
                user.Jobs = await _freelancingPlatform.JobManager.GetAll()
                    .Where(j => !(j.IsClosed && j.IsPaymentDenied) && j.Employer.Id == user.Id || j.HiredFreelancer.Id == user.Id)
                    .ToListAsync();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
#if AllowUserDeletion
            var deletedUser = await _userManager.FindByIdAsync(userId);
            if (deletedUser is null)
                return UnprocessableEntity(userId);

            await _userManager.DeleteAsync(deletedUser);
#endif

            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var blockedUser = await _userManager.FindByIdAsync(userId);
            if (blockedUser is null)
                return UnprocessableEntity(userId);

            if (!blockedUser.IsBlocked)
            {
                blockedUser.IsBlocked = true;
                await _userManager.UpdateAsync(blockedUser);
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var blockedUser = await _userManager.FindByIdAsync(userId);
            if (blockedUser is null)
                return UnprocessableEntity(userId);

            if (blockedUser.IsBlocked)
            {
                blockedUser.IsBlocked = false;
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
    }
}
