using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Infrastructure.Implements.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CountUnreadNotificationsAsync(int recipientId)
        {
            return await _unitOfWork.Notifications.GetAllAsync()
                        .CountAsync(n => n.RecipientId == recipientId && !n.IsRead);
        }

        public async Task<Notification> CreateNotificationAsync(int recipientId, int eventId, string message)
        {
            var notification = new Notification
            {
                RecipientId = recipientId,
                EventId = eventId,
                Message = message,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _unitOfWork.Notifications.Add(notification);
            await _unitOfWork.CompleteAsync();

            return notification;
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _unitOfWork.Notifications.GetAllAsync()
                .Include(n => n.Event)
                    .ThenInclude(e => e.Lab)
                .Include(n => n.Event)
                    .ThenInclude(e => e.Zone)
                .Include(n => n.Event)
                    .ThenInclude(e => e.Organizer)
                .Where(n => n.RecipientId == userId)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null)
                return false;

            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
            
            return await _unitOfWork.CompleteAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _unitOfWork.Notifications.GetAllAsync()
                .CountAsync(n => n.RecipientId == userId && !n.IsRead);
        }
    }
}
