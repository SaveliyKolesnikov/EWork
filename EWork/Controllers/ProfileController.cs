using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using EWork.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EWork.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IOptions<PhotoConfig> _photoOptions;

        public ProfileController(UserManager<User> userManager, IOptions<PhotoConfig> photoOptions)
        {
            _userManager = userManager;
            _photoOptions = photoOptions;
        }

        public async Task<IActionResult> Profile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
                return BadRequest();

            var profileViewModel = new ProfileViewModel(user, _photoOptions.Value.UsersPhotosPath);
            return View(profileViewModel);
        }
    }
}