using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EWork.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageManager _messageManager;
        private readonly IHostingEnvironment _environment;
        private readonly IOptions<PhotoConfig> _photoOptions;

        public ChatController(UserManager<User> userManager,
            IMessageManager messageManager,
            IHostingEnvironment environment,
            IOptions<PhotoConfig> photoOptions)
        {
            _userManager = userManager;
            _messageManager = messageManager;
            _environment = environment;
            _photoOptions = photoOptions;
        }

        public async Task<IActionResult> Index(string recieverUsername)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var reciever = string.IsNullOrWhiteSpace(recieverUsername) ?
                null : await _userManager.FindByNameAsync(recieverUsername);
            var currentUserId = _userManager.GetUserId(User);
            var messages = _messageManager.GetAll()
                .Where(m => m.Receiver.Id == currentUserId || m.Sender.Id == currentUserId)
                .OrderBy(m => m.SendDate);
            var pathToProfilePhotos = Path.Combine(_environment.ContentRootPath, _photoOptions.Value.UsersPhotosPath);

            var chatViewModel = new ChatViewModel(currentUser, messages, pathToProfilePhotos, reciever);
            return View(chatViewModel);
        }
    }
}