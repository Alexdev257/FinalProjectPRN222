using FPP.Application.DTOs;
using FPP.Application.DTOs.LabEvent;
using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Infrastructure.Implements.Services
{
    public class LabEventService : ILabEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LabEventService> _logger; // Inject Logger

        public LabEventService(IUnitOfWork unitOfWork, ILogger<LabEventService> logger) // Add ILogger
        {
            _unitOfWork = unitOfWork;
            _logger = logger; // Assign logger
        }

        public async Task<LabEvent?> GetEventByIdAsync(int eventId)
        {
            // Include related data needed for display or logic
            return await _unitOfWork.LabEvents.GetAllAsync()
                           .Include(e => e.Lab)
                           .Include(e => e.Zone)
                           .Include(e => e.ActivityType)
                           .Include(e => e.Organizer) // If needed
                           .FirstOrDefaultAsync(e => e.EventId == eventId);
        }

        public async Task<bool> CancelEventAsync(int eventId, int userId)
        {
            var labEvent = await _unitOfWork.LabEvents.GetAllAsync()
                                    .Include(e => e.EventParticipants) // Need participants to check
                                    .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (labEvent == null) return false; // Event not found

            // Allow cancellation only if upcoming and user is organizer or participant
            // AND status is pending or approved (adjust logic as needed)
            bool isParticipantOrOrganizer = labEvent.OrganizerId == userId || labEvent.EventParticipants.Any(ep => ep.UserId == userId);
            bool isCancellableStatus = labEvent.Status.ToLower() == "pending" || labEvent.Status.ToLower() == "approved";

            if (labEvent.StartTime > DateTime.Now && isParticipantOrOrganizer && isCancellableStatus)
            {
                labEvent.Status = "Cancelled";
                _unitOfWork.LabEvents.Update(labEvent); // Mark for update
                return await _unitOfWork.CompleteAsync(); // Save changes
            }
            return false; // Cannot cancel
        }

        public Task<List<LabEventVM>> GetEventsForScheduleAsync(DateTime selectedDate, int? filterLabId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckTimeConflictAsync(int labId, int zoneId, DateTime startTime, DateTime endTime)
        {
            return await _unitOfWork.LabEvents.GetAllAsync()
                .AnyAsync(e => e.LabId == labId
                              && e.ZoneId == zoneId
                              && (e.Status.ToLower() == "approved" || e.Status.ToLower() == "pending")
                              && e.StartTime < endTime // Existing starts before new ends
                              && e.EndTime > startTime); // Existing ends after new starts
        }

        public async Task<(bool Success, string ErrorMessage)> CreateBookingAsync(BookingInputModel bookingInput, int organizerUserId)
        {
            DateTime startDateTime;
            DateTime endDateTime;
            try
            {
                startDateTime = bookingInput.BookingDate.Date + bookingInput.StartTime;
                endDateTime = bookingInput.BookingDate.Date + bookingInput.EndTime;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, "Error combining date and time during booking creation.");
                return (false, "Invalid date/time combination.");
            }


            // Double-check for conflicts right before saving (optional but safer)
            bool hasConflict = await CheckTimeConflictAsync(bookingInput.LabId, bookingInput.ZoneId, startDateTime, endDateTime);
            if (hasConflict)
            {
                return (false, "The selected time slot became unavailable. Please choose another time.");
            }

            var newEvent = new LabEvent
            {
                LabId = bookingInput.LabId,
                ZoneId = bookingInput.ZoneId,
                ActivityTypeId = bookingInput.ActivityTypeId,
                OrganizerId = organizerUserId,
                Title = bookingInput.Title,
                Description = bookingInput.Description,
                StartTime = startDateTime,
                EndTime = endDateTime,
                Status = "Pending", // Default status
                CreatedAt = DateTime.UtcNow
            };

            // Add the Event
            _unitOfWork.LabEvents.Add(newEvent);

            // Create the Participant entry
            // EF Core *should* link this automatically if navigation properties are set correctly,
            // but explicitly creating it is safer.
            var participant = new EventParticipant
            {
                // EventId will be set by EF Core if relationship is configured
                // Or you might need to save Event first, then add participant. Let's try direct add.
                Event = newEvent, // Link via navigation property
                UserId = organizerUserId,
                Role = 1 // Assuming Role 1 is Organizer/Participant
            };
            _unitOfWork.EventParticipants.Add(participant); // Add participant

            try
            {
                // Save both Event and Participant in one transaction
                bool saved = await _unitOfWork.CompleteAsync();
                if (saved)
                {
                    return (true, string.Empty); // Success
                }
                else
                {
                    _logger.LogWarning("CompleteAsync returned false when creating booking for User {UserId}", organizerUserId);
                    return (false, "Failed to save booking request to the database.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while saving booking for User {UserId}", organizerUserId);
                // Consider more specific error handling/logging if needed
                return (false, "An unexpected error occurred while saving the booking.");
            }
        }

        public async Task<List<LabEvent>> GetAllBookingsWithDetailsAsync()
        {
            return await _unitOfWork.LabEvents.GetAllAsync()
            .Include(e => e.Lab)
            .Include(e => e.Zone)
            .Include(e => e.Organizer)
            .Include(e => e.ActivityType)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
        }

        public async Task<LabEvent?> GetBookingByIdAsync(int eventId)
        {
            return await _unitOfWork.LabEvents.GetAllAsync()
                .Include(e => e.Lab)
                .Include(e => e.Zone)
                .Include(e => e.Organizer)
                .Include(e => e.ActivityType)
                .FirstOrDefaultAsync(e => e.EventId == eventId);
        }

        public async Task<bool> UpdateBookingStatusAsync(int eventId, string status)
        {
            var booking = await _unitOfWork.LabEvents.GetByIdAsync(eventId);
            if (booking == null) return false;

            booking.Status = status;
            return await _unitOfWork.CompleteAsync();
        }
    }
}
