﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using EWork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EWork.Config;

namespace EWork.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<PhotoConfig> _photoOptions;

        public enum UserStatus
        {
            Employeer,
            Freelancer
        }

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IOptions<PhotoConfig> photoOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _photoOptions = photoOptions;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(24, MinimumLength = 2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [StringLength(24, MinimumLength = 2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
            [Display(Name = "Surname")]
            public string Surname { get; set; }

            [Required]
            [StringLength(24, MinimumLength = 2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Range(0, 1)]
            public UserStatus UserStatus { get; set; }
        }

        public void OnGet(string returnUrl = null) => ReturnUrl = returnUrl;


        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                User user = null;

                switch (Input.UserStatus)
                {
                    case UserStatus.Freelancer:
                        user = new Freelancer { Proposals = new List<Proposal>() };
                        break;
                    case UserStatus.Employeer:
                        user = new Employer();
                        break;
                }

                if (user is null)
                    return Page();

                user.Name = Input.Name;
                user.Surname = Input.Surname;
                user.UserName = Input.Username;
                user.Email = Input.Email;
                user.ProfilePhotoName = _photoOptions.Value.DefaultPhoto;
                user.Balance = new Balance();
                user.Jobs = new List<Job>();
                user.Reviews = new List<Review>();
                user.SingUpDate = DateTime.Now;
                user.Notifications = new List<Notification>();
                

                IdentityResult result = null;

                try
                {
                    result = await _userManager.CreateAsync(user, Input.Password);
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException.Message.Contains("Email"))
                    {
                        Trace.WriteLine(e.Message);
                        ModelState.AddModelError(string.Empty, "Email is already taken");
                        return Page();
                    }

                    ExceptionDispatchInfo.Capture(e).Throw();
                }

                await _userManager.AddToRoleAsync(user, user.Role);
                if (result is null)
                    return Page();

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
