using System;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using EWork.Config;
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
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly IOptions<PhotoConfig> _photoOptions;

        public ProfileController(UserManager<User> userManager, IFreelancingPlatform freelancingPlatform, IOptions<PhotoConfig> photoOptions)
        {
            _userManager = userManager;
            _freelancingPlatform = freelancingPlatform;
            _photoOptions = photoOptions;
        }

        public async Task<IActionResult> Profile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return NotFound(username);

            var currentUser = await _userManager.GetUserAsync(User);
            var isCurrentUserCanAddReview =
                await IsUserCanAddReviewAsync(reviewedUser: user, senderOfReview: currentUser);

            user.Reviews = await _freelancingPlatform.ReviewManager.GetAll().Where(r => r.User.Id == user.Id).ToListAsync();
            var profileViewModel = new ProfileViewModel(user, currentUser, _photoOptions.Value.UsersPhotosPath, isCurrentUserCanAddReview);
            return View(profileViewModel);
        }

        public async Task<IActionResult> AddReview(string nameOfReviewedUser, Review review)
        {
            if (nameOfReviewedUser is null)
                throw new ArgumentNullException(nameof(nameOfReviewedUser));

            if (review is null)
                throw new ArgumentNullException(nameof(review));

            var reviewedUser = await _userManager.FindByNameAsync(nameOfReviewedUser) ??
                               throw new ArgumentException($"User with user name {nameOfReviewedUser} doesn't exist.");

            var currentUser = await _userManager.GetUserAsync(User) ?? throw new AuthenticationException();
            if (!await IsUserCanAddReviewAsync(reviewedUser: reviewedUser, senderOfReview: currentUser))
                return Forbid();

            if (ModelState.IsValid)
            {
                review.SendDate = DateTime.Now;
                review.User = reviewedUser;
                review.Sender = currentUser;

                await _freelancingPlatform.ReviewManager.AddAsync(review);

                return RedirectToAction("Profile", new { username = nameOfReviewedUser });
            }

            var errors = new StringBuilder();
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errors.AppendLine(error.ErrorMessage);
                }
            }
            return Content(errors.ToString());
        }

        public async Task<IActionResult> UpdateReview(string nameOfReviewedUser, Review review)
        {
            if (nameOfReviewedUser is null)
                throw new ArgumentNullException(nameof(nameOfReviewedUser));

            if (review is null)
                throw new ArgumentNullException(nameof(review));

            if (ModelState.IsValid)
            {
                var reviewFromDb = await _freelancingPlatform.ReviewManager
                .FindAsync(r => r.Id == review.Id) ??
                      throw new ArgumentException($"User hasn't sent a review");

                reviewFromDb.Text = review.Text;
                reviewFromDb.Value = review.Value;

                var reviewedUser = await _userManager.FindByNameAsync(nameOfReviewedUser) ??
                                   throw new ArgumentException($"User with user name {nameOfReviewedUser} doesn't exist.");

                var currentUser = await _userManager.GetUserAsync(User) ?? throw new AuthenticationException();
                if (!await IsUserCanAddReviewAsync(reviewedUser: reviewedUser, senderOfReview: currentUser))
                    return Forbid();


                review.SendDate = DateTime.Now;
                await _freelancingPlatform.ReviewManager.UpdateAsync(review);

                return RedirectToAction("Profile", new { username = nameOfReviewedUser });
            }

            var errors = new StringBuilder();
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errors.AppendLine(error.ErrorMessage);
                }
            }
            return Content(errors.ToString());
        }

        private async Task<bool> IsUserCanAddReviewAsync(User reviewedUser, User senderOfReview)
        {
            switch (reviewedUser)
            {
                case Employer _ when senderOfReview is Freelancer:
                    return await _freelancingPlatform.JobManager.GetAll().AnyAsync(j =>
                        j.IsClosed && j.Employer.Id == reviewedUser.Id && j.HiredFreelancer.Id == senderOfReview.Id);
                case Freelancer _ when senderOfReview is Employer:
                    return await _freelancingPlatform.JobManager.GetAll().AnyAsync(j =>
                        j.IsClosed && j.Employer.Id == senderOfReview.Id && j.HiredFreelancer.Id == reviewedUser.Id);
                default:
                    return false;
            }
        }


    }
}