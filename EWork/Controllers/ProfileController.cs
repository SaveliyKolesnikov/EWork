using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EWork.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IReviewManager _reviewManager;
        private readonly IOptions<PhotoConfig> _photoOptions;

        public ProfileController(UserManager<User> userManager, IReviewManager reviewManager, IOptions<PhotoConfig> photoOptions)
        {
            _userManager = userManager;
            _reviewManager = reviewManager;
            _photoOptions = photoOptions;
        }

        public async Task<IActionResult> Profile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return BadRequest();

            user.Reviews = await _reviewManager.GetAll().Where(r => r.User.Id == user.Id).ToListAsync();
            var profileViewModel = new ProfileViewModel(user, _photoOptions.Value.UsersPhotosPath);
            return View(profileViewModel);
        }
    }
}