using FPP.Application.DTOs;
using FPP.Application.DTOs.LabEvent;
using FPP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IServices
{
    public interface ILabEventService
    {
        Task<LabEvent?> GetEventByIdAsync(int eventId);
        Task<bool> CancelEventAsync(int eventId, int userId);
        Task<List<LabEventVM>> GetEventsForScheduleAsync(DateTime selectedDate, int? filterLabId);
        Task<bool> CheckTimeConflictAsync(int labId, int zoneId, DateTime startTime, DateTime endTime);
        Task<(bool Success, string ErrorMessage)> CreateBookingAsync(BookingInputModel bookingInput, int organizerUserId);
        Task<List<LabEvent>> GetAllBookingsWithDetailsAsync();
        Task<LabEvent?> GetBookingByIdAsync(int eventId);
        Task<bool> UpdateBookingStatusAsync(int eventId, string status);
    }
}
