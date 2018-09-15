using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.Services.Mappers.Interfaces;
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

            var messages = _messageManager.GetAll()
                .Where(m => m.Receiver.Id == currentUser.Id || m.Sender.Id == currentUser.Id)
                .OrderBy(m => m.SendDate);
            var pathToProfilePhotos = Path.Combine(_environment.ContentRootPath, _photoOptions.Value.UsersPhotosPath);

            var chatViewModel = new ChatViewModel(currentUser, messages, pathToProfilePhotos, receiver);
            return View(chatViewModel);
        }

        [Authorize(Roles = "moderator, administrator")]
        public IActionResult Dialog(string username1, string username2)
        {
            var chatHistory = _messageManager.GetChatHistory(username1, username2).OrderBy(m => m.SendDate);
            return View(chatHistory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetMessages(string username1, string username2)
        {
            var currentUserName = _userManager.GetUserName(User);
            if (!(currentUserName == username1 || currentUserName == username2 ||
                User.IsInRole("moderator") || User.IsInRole("administrator")))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new {message = "Authorization error."});
            }

            var chat = await _messageManager.GetChatHistory(username1, username2).OrderBy(m => m.SendDate).ToArrayAsync();
            var jsonChat = _messageMapper.MapRange(chat);

            return Json(jsonChat, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}