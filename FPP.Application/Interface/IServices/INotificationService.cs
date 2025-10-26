using FPP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IServices
{
    public interface INotificationService
    {
        Task<int> CountUnreadNotificationsAsync(int recipientId);
        Task<Notification> CreateNotificationAsync(int recipientId, int eventId, string message);
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}
