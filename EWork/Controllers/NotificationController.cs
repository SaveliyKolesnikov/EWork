using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using EWork.Services.Interfaces;
using EWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EWork.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;
        private readonly int _takeAmount;

        public NotificationController(IFreelancingPlatform freelancingPlatform, 
            UserManager<User> userManager,
            IOptions<FreelancingPlatformConfig> fpConfig)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
            _takeAmount = fpConfig.Value.TakeAmount;
        }

        public IActionResult UserNotificationsPage()
        {
            ViewBag.TakeAmount = _takeAmount;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var user = await _userManager.GetUserAsync(User);
            var notification = await _freelancingPlatform.NotificationManager.FindAsync(n => n.Id == notificationId);

            if (notification is null)
                return UnprocessableEntity(notificationId);

            if (notification.Receiver.Id != user.Id)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Authorization error." });
            }

            await _freelancingPlatform.NotificationManager.DeleteNotificationAsync(user, notification);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetNotifications(int skipAmount, int takeAmount, string receiverUserName)
        {
            if (_userManager.GetUserName(User) != receiverUserName)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Authorization error." });
            }

            var result = _freelancingPlatform.NotificationManager.GetAll()
                .Where(n => n.Receiver.UserName == receiverUserName)
                .Skip(skipAmount).Take(takeAmount)
                .Select(n => new { n.Id, n.Title, n.Source, n.CreatedDate });

            return Json(result, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
