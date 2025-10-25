using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
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
    }
}
