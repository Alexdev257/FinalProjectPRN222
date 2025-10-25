using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FPP.Presentation.Pages
{
    [Authorize] // Require login to access this page
    public class MyBookingsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork; // Inject UoW
        private readonly ILabEventService _labEventService; // Inject Event Service for cancelling
        private readonly IUserService _userService; // To get current user info for display


        public MyBookingsModel(IUnitOfWork unitOfWork, ILabEventService labEventService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _labEventService = labEventService;
            _userService = userService;
        }

        public User CurrentUser { get; set; }
        public List<BookingViewModel> UpcomingBookings { get; set; } = new List<BookingViewModel>();
        public List<BookingViewModel> PastBookings { get; set; } = new List<BookingViewModel>();

        // ViewModel for displaying booking details
        public class BookingViewModel
        {
            public int EventId { get; set; }
            public string LabName { get; set; } = string.Empty;
            public string ZoneName { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string ActivityTypeName { get; set; } = string.Empty;
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string Status { get; set; } = string.Empty;
            public bool CanCancel { get; set; } // Flag to show/hide cancel button
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return RedirectToPage("/Login"); // Should not happen if [Authorize] works
            }

            CurrentUser = await _userService.GetById(userId);
            if (CurrentUser == null)
            {
                // Handle case where user exists in auth cookie but not DB (e.g., deleted)
                return RedirectToPage("/Login"); // Or show an error
            }


            var now = DateTime.Now;

            // Fetch all bookings where the user is a participant or organizer
            // Use IUnitOfWork directly here as IEventParticipantService might not have this specific query
            var userBookings = await _unitOfWork.EventParticipants.GetAllAsync()
                .Where(ep => ep.UserId == userId)
                .Include(ep => ep.Event) // Include the LabEvent details
                    .ThenInclude(e => e.Lab) // Then include Lab from LabEvent
                .Include(ep => ep.Event)
                    .ThenInclude(e => e.Zone) // Then include Zone from LabEvent
                .Include(ep => ep.Event)
                    .ThenInclude(e => e.ActivityType) // Then include ActivityType
                .Select(ep => ep.Event) // Select the actual LabEvent
                .OrderByDescending(e => e.StartTime) // Order by start time (most recent first)
                .ToListAsync();

            foreach (var booking in userBookings)
            {
                var viewModel = new BookingViewModel
                {
                    EventId = booking.EventId,
                    LabName = booking.Lab?.Name ?? "N/A", // Use null conditional
                    ZoneName = booking.Zone?.Name ?? "N/A",
                    Title = booking.Title,
                    ActivityTypeName = booking.ActivityType?.Name ?? "N/A",
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    Status = booking.Status
                };

                // Determine if upcoming or past and if cancellable
                if (booking.EndTime > now)
                {
                    viewModel.CanCancel = (booking.Status.ToLower() == "pending" || booking.Status.ToLower() == "approved") && booking.StartTime > now;
                    UpcomingBookings.Add(viewModel);
                }
                else
                {
                    viewModel.CanCancel = false; // Cannot cancel past events
                    PastBookings.Add(viewModel);
                }
            }

            // Optional: Sort upcoming bookings ascending if needed (they are currently descending)
            UpcomingBookings = UpcomingBookings.OrderBy(b => b.StartTime).ToList();


            return Page();
        }

        // Handler for the Cancel button
        public async Task<IActionResult> OnPostCancelAsync(int eventId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return RedirectToPage("/Login");
            }

            bool cancelled = await _labEventService.CancelEventAsync(eventId, userId);

            if (cancelled)
            {
                // Optional: Add a success message via TempData
                TempData["StatusMessage"] = "Booking cancelled successfully.";
            }
            else
            {
                // Optional: Add an error message via TempData
                TempData["ErrorMessage"] = "Could not cancel booking. It may have already started or you don't have permission.";
            }

            return RedirectToPage(); // Reload the MyBookings page
        }
    }
}
