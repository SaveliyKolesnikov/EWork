using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EWork.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IFreelancingPlatform _freelancingPlatform;
        private readonly UserManager<User> _userManager;

        public NotificationController(IFreelancingPlatform freelancingPlatform, UserManager<User> userManager)
        {
            _freelancingPlatform = freelancingPlatform;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> UserNotificationsPage()
        {
            var user = await _userManager.GetUserAsync(User);
            var userNotifications =
                _freelancingPlatform.NotificationManager.GetAll().Where(n => n.Receiver.Id == user.Id);
            
            return View(userNotifications);
        }

        [Authorize]
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

    }
}
