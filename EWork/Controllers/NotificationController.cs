using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EWork.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;

        public NotificationController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
        }

        public async Task<IActionResult> UserNotificationsPage()
        {
            var user = await _userManager.GetUserAsync(User);
            var userNotifications =
                _freelancingPlatform.NotificationManager.GetAll().Where(n => n.Receiver.Id == user.Id);
            var notificationViewModel = new NotificationViewModel(user, userNotifications);
            return View(notificationViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var user = await _userManager.GetUserAsync(User);
            var notification = await _freelancingPlatform.NotificationManager.FindAsync(n => n.Id == notificationId);

            if (notification is null)
                return BadRequest();

            await _freelancingPlatform.NotificationManager.DeleteNotificationAsync(user, notification);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetNotifications(int skipAmount, int takeAmount, string receiverUserName)
        {
            if (_userManager.GetUserName(User) != receiverUserName)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Authorization error." });
            }

            var result = _freelancingPlatform.NotificationManager.GetAll()
                .Where(n => n.Receiver.UserName == receiverUserName)
                .Skip(skipAmount).Take(takeAmount)
                .Select(n => new { n.Id, n.Title, n.Source, n.CreatedDate });
            return Json(result);
        }
    }
}
