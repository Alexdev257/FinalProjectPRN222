using FPP.Application.DTOs.LabEvent;
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

        public async Task<Dictionary<int, List<BookingCalendarItem>>> GetUserBookingsGroupedByDayAsync(int userId, int year, int month)
        {
            var userBookings = await _unitOfWork.EventParticipants.GetAllAsync()
                .Where(ep => ep.UserId == userId
                             && ep.Event.StartTime.Year == year
                             && ep.Event.StartTime.Month == month
                             && (ep.Event.Status.ToLower() == "approved" || ep.Event.Status.ToLower() == "pending"))
                .Include(ep => ep.Event)
                    .ThenInclude(e => e.Lab)
                .Select(ep => ep.Event) // Select the LabEvent
                .OrderBy(e => e.StartTime) // Optional: order before grouping if needed downstream
                .ToListAsync(); // Fetch the data

            // Group in memory after fetching
            var groupedBookings = userBookings
                .GroupBy(e => e.StartTime.Day)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => new BookingCalendarItem
                    {
                        EventId = e.EventId,
                        LabName = e.Lab?.Name ?? "N/A",
                        StartTime = e.StartTime
                    }).ToList()
                );

            return groupedBookings;
        }
    }
}
