using GraduateProject_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface INotificationRepository
    {
        Task<bool> CreateNotificationAsync(Notification notification);
        Task<bool> DeleteNotificationAsync(int notificationId);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> SendNotificationAsync(Notification notification);
    }
}
