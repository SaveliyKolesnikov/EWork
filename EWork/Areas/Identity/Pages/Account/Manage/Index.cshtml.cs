using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EWork.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public partial class IndexModel : PageModel
    {
        private readonly IHostingEnvironment _env;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IBalanceManager _balanceManager;
        private readonly IOptions<PhotoConfig> _photoConfig;

        private string UsersPhotosPath =>
            Path.Combine(_env.ContentRootPath, _photoConfig.Value.UsersPhotosPath);


        public IndexModel(
            IHostingEnvironment env,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            IBalanceManager balanceManager,
            IOptions<PhotoConfig> photoConfig)
        {
            _env = env;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _balanceManager = balanceManager;
            _photoConfig = photoConfig;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }


        public IEnumerable<string> AllowedExtensions => _photoConfig.Value.AllowedExtensions;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [StringLength(4096, ErrorMessage = "{0} length must be less then 4096")]
            [Display(Name = "Overview")]
            public string Description { get; set; }

            public IFormFile UploadedImage { get; set; }
            public string ProfilePhotoName { get; set; }

            [DataType(DataType.Currency)]
            [Range(0, double.MaxValue)]
            [Display(Name = "Amount of replenishment")]
            public decimal AmountOfReplenishment { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var usersPhotosPath = Path.Combine("/images", "UsersPhotos");

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var description = user.Description;
            var profilePhotoName = user.ProfilePhotoName ?? _photoConfig.Value.DefaultPhoto;
            var profilePhotoUrl = Path.Combine(usersPhotosPath, profilePhotoName);

            Username = userName;

            Input = new InputModel
            {
                Email = email,
                PhoneNumber = phoneNumber,
                Description = description,
                ProfilePhotoName = profilePhotoUrl,
                AmountOfReplenishment = 0
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                IdentityResult setEmailResult = null;
                try
                {
                    setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException.Message.Contains("Email"))
                    {
                        Trace.WriteLine(e.Message);
                        ModelState.AddModelError(string.Empty, "Email is already taken");
                        return await OnGetAsync();
                    }

                    ExceptionDispatchInfo.Capture(e).Throw();
                }

                if (setEmailResult is null || !setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            if (Input.Description != user.Description)
            {
                user.Description = Input.Description;
                var setUserDescription = await _userManager.UpdateAsync(user);
                if (!setUserDescription.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting user description for user with ID '{userId}'.");
                }
            }

            var curUserBalance = await _balanceManager.FindAsync(b => b.UserId == user.Id);
            if (Input.AmountOfReplenishment > 0)
            {
                await _balanceManager.ReplenishBalanceAsync(curUserBalance, Input.AmountOfReplenishment);
            }

            if (!(Input.UploadedImage is null || Input.UploadedImage.Length == 0))
            {
                var maxFileSize = _photoConfig.Value.MaxSize;
                if (maxFileSize < Input.UploadedImage.Length)
                {
                    ModelState.AddModelError(string.Empty, $"The profile picture size must be less than {BytesToMegabytes(maxFileSize)} mb.");
                    return await OnGetAsync();
                }

                var fileExtension = Path.GetExtension(Input.UploadedImage.FileName);
                if (_photoConfig.Value.AllowedExtensions.All(e => e != fileExtension))
                {
                    var allowedExtensionsString = string.Join("/", _photoConfig.Value.AllowedExtensions);
                    ModelState.AddModelError(string.Empty, $"The profile picture must have an extension of {allowedExtensionsString}.");
                    return await OnGetAsync();
                }

                var usersPhotosPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                    "images", "UsersPhotos");

                if (Directory.Exists(usersPhotosPath))
                {
                    var previousPhoto = user.ProfilePhotoName;
                    if (!(previousPhoto is null) && previousPhoto != _photoConfig.Value.DefaultPhoto)
                    {
                        var previousPhotoPath = Path.Combine(usersPhotosPath, previousPhoto);

                        try
                        {
                            System.IO.File.Delete(previousPhotoPath);
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine(e.Message);
                            ExceptionDispatchInfo.Capture(e).Throw();
                        }
                    }

                    var newImageName = user.UserName + "_" + "profile_photo" + fileExtension;
                    var pathToNewImage = Path.Combine(usersPhotosPath, newImageName);



                    using (var inputStream = new MemoryStream())
                    {
                        await Input.UploadedImage.CopyToAsync(inputStream);
                        using (var temp = new Bitmap(inputStream))
                        {
                            Image image = new Bitmap(temp);

                            if (image.Width > image.Height)
                            {
                                var startX = (image.Width - image.Height) / 2;
                                const int startY = 0;
                                var sourceSize = image.Height;
                                var res = CropImage(image, startX, startY, sourceSize, sourceSize, sourceSize, sourceSize);
                                res.Save(pathToNewImage);
                            }
                            else if (image.Width < image.Height)
                            {
                                const int startX = 0;
                                var startY = (image.Height - image.Width) / 2; 
                                var sourceSize = image.Width;
                                var res = CropImage(image, startX, startY, sourceSize, sourceSize, sourceSize, sourceSize);
                                res.Save(pathToNewImage);
                            }
                            else
                            {
                                image.Save(pathToNewImage);
                            }

                            Image CropImage(Image sourceImage, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
                            {
                                Image destinationImage = new Bitmap(destinationWidth, destinationHeight);
                                var g = Graphics.FromImage(destinationImage);

                                g.DrawImage(
                                    sourceImage,
                                    new Rectangle(0, 0, destinationWidth, destinationHeight),
                                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                                    GraphicsUnit.Pixel
                                );

                                return destinationImage;
                            }
                        }
                    }

                    user.ProfilePhotoName = newImageName;
                    await _userManager.UpdateAsync(user);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();

            double BytesToMegabytes(long bytes) =>
                (double)bytes / 1_000_000;
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}

