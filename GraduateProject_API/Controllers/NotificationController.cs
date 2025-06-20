
using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDTO dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var result = await _notificationRepository.CreateNotificationAsync(notification);
            if (!result)
                return BadRequest("Failed to create notification.");

            return Ok(new
            {
                notification.NotificationID,
                notification.UserId,
                notification.Title,
                notification.Message,
                notification.CreatedAt
            });
        }


        //[HttpGet("User/{userId}")]
        //public async Task<IActionResult> GetUserNotifications(int userId)
        //{
        //    var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
        //    return Ok(notifications);
        //}

        [HttpGet("ByUser/{userId}")]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }


        [HttpPut("MarkAsRead/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var result = await _notificationRepository.MarkAsReadAsync(notificationId);
            return result ? Ok(new { Message = "Marked as read." }) : NotFound();
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var result = await _notificationRepository.DeleteNotificationAsync(notificationId);
            return result ? Ok(new { Message = "Notification deleted." }) : NotFound();
        }
    }
}
