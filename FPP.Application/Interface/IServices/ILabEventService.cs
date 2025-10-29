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
        Task<(bool Success, string ErrorMessage, int? eventId)> CreateBookingAsync(BookingInputModel bookingInput, int organizerUserId);
        Task<List<LabEvent>> GetAllBookingsWithDetailsAsync();
        Task<LabEvent?> GetBookingByIdAsync(int eventId);
        Task<bool> UpdateBookingStatusAsync(int eventId, string status);

        Task<LabEvent?> GetBookingWithDetailsAsync(int eventId);
        Task<List<SecurityLog>> GetSecurityLogsByEventIdAsync(int eventId);
        Task<List<EventParticipant>> GetEventParticipantsAsync(int eventId);

        Task<List<SecurityLog>> GetAllSecurityLogsWithDetailsAsync();
        Task<List<SecurityLog>> GetSecurityLogsByLabIdAsync(int labId);
        Task<List<SecurityLog>> GetPendingSecurityLogsAsync();
        Task<SecurityLog?> GetSecurityLogByIdAsync(int logId);
        Task<bool> UpdateSecurityLogNotesAsync(int logId, string notes);
        Task<bool> AcknowledgeSecurityLogAsync(int logId, int managerId, string note);

    }
}
