using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EWork.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageManager _messageManager;
        private readonly IHostingEnvironment _environment;
        private readonly IOptions<PhotoConfig> _photoOptions;
        private readonly IMessageMapper _messageMapper;

        public ChatController(UserManager<User> userManager,
            IMessageManager messageManager,
            IHostingEnvironment environment,
            IOptions<PhotoConfig> photoOptions,
            IMessageMapper messageMapper)
        {
            _userManager = userManager;
            _messageManager = messageManager;
            _environment = environment;
            _photoOptions = photoOptions;
            _messageMapper = messageMapper;
        }

        public async Task<IActionResult> Index(string receiverUsername)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var receiver = string.IsNullOrWhiteSpace(receiverUsername) ?
                null : await _userManager.FindByNameAsync(receiverUsername);
            var currentUserId = _userManager.GetUserId(User);
            var messages = _messageManager.GetAll()
                .Where(m => m.Receiver.Id == currentUserId || m.Sender.Id == currentUserId)
                .OrderBy(m => m.SendDate);
            var pathToProfilePhotos = Path.Combine(_environment.ContentRootPath, _photoOptions.Value.UsersPhotosPath);

            var chatViewModel = new ChatViewModel(currentUser, messages, pathToProfilePhotos, receiver);
            return View(chatViewModel);
        }

        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetMessages(string username1, string username2)
        {
            var currentUserName = _userManager.GetUserName(User);
            if (!(currentUserName == username1 || currentUserName == username2 ||
                User.IsInRole("moderator") || User.IsInRole("administrator")))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new {message = "Authorization error."});
            }

            var chat = await _messageManager.GetChatHistory(username1, username2).ToArrayAsync();
            var jsonChat = _messageMapper.MapRange(chat);

            return Json(jsonChat, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }
    }
}