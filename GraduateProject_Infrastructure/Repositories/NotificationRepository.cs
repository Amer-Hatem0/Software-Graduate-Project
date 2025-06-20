using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GraduateProject_Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        public async Task<bool> CreateNotificationAsync(Notification notification)
        {
            try
            {
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateNotificationAsync Error: " + ex.Message);
                return false;
            }
        }



        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SendNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
