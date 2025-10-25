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
    public class EventParticipantService : IEventParticipantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventParticipantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CountUpcomingBookingsForUserAsync(int userId)
        {
            var now = DateTime.Now;
            // Cần Include LabEvent để lọc theo StartTime
            return await _unitOfWork.EventParticipants.GetAllAsync()
                        .Include(ep => ep.Event) // Include thông tin sự kiện liên quan
                        .CountAsync(ep => ep.UserId == userId && ep.Event.StartTime >= now);
        }
    }
}
