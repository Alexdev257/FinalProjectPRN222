using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using FPP.Presentation.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FPP.Presentation.Pages
{
    [Authorize]
    public class ManagerDashboardModel : PageModel
    {
        private readonly ILabEventService _labEventService;
        private readonly IUserService _userService;
        private readonly IHubContext<NotificationBookingHub> _hubContext;

        public ManagerDashboardModel(
            ILabEventService labEventService,
            IUserService userService,
            IHubContext<NotificationBookingHub> hubContext)
        {
            _labEventService = labEventService;
            _userService = userService;
            _hubContext = hubContext;
        }
        [BindProperty]
        public User? CurrentUser { get; set; }
        [BindProperty]
        public List<LabEvent> Bookings { get; set; } = new();
        [BindProperty]
        public int PendingCount { get; set; }
        [BindProperty]
        public int ApprovedCount { get; set; }
        [BindProperty]
        public int RejectedCount { get; set; }
        [BindProperty]
        public int TotalCount { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return RedirectToPage("/Login");

            CurrentUser = await _userService.GetById(userId);
            if (CurrentUser == null || CurrentUser.Role != 2) // Only managers
                return RedirectToPage("/Login");

            // Load all bookings with related data
            Bookings = await _labEventService.GetAllBookingsWithDetailsAsync();

            // Calculate stats
            var today = DateTime.Today;
            PendingCount = Bookings.Count(b => b.Status == "Pending");
            ApprovedCount = Bookings.Count(b => b.Status == "Approved" && b.CreatedAt.Date == today);
            RejectedCount = Bookings.Count(b => b.Status == "Rejected" && b.CreatedAt.Date == today);
            TotalCount = Bookings.Count;

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int eventId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            CurrentUser = await _userService.GetById(userId);
            if (CurrentUser == null || CurrentUser.Role != 2)
                return new JsonResult(new { success = false, message = "Unauthorized" });

            var success = await _labEventService.UpdateBookingStatusAsync(eventId, "Approved");

            if (success)
            {
                // Get booking details
                var booking = await _labEventService.GetBookingByIdAsync(eventId);

                // Send notification to student
                await SendNotificationToStudent(booking, "Approved");

                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false, message = "Failed to approve booking" });
        }

        public async Task<IActionResult> OnPostRejectAsync(int eventId, string reason)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            CurrentUser = await _userService.GetById(userId);
            if (CurrentUser == null || CurrentUser.Role != 2)
                return new JsonResult(new { success = false, message = "Unauthorized" });

            var success = await _labEventService.UpdateBookingStatusAsync(eventId, "Rejected");

            if (success)
            {
                // Get booking details
                var booking = await _labEventService.GetBookingByIdAsync(eventId);

                // Send notification to student with reason
                await SendNotificationToStudent(booking, "Rejected", reason);

                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false, message = "Failed to reject booking" });
        }

        private async Task SendNotificationToStudent(LabEvent booking, string status, string? reason = null)
        {
            try
            {
                var studentId = booking.OrganizerId;
                var connectionId = NotificationBookingHub.GetConnectionId(studentId.ToString());

                if (string.IsNullOrEmpty(connectionId))
                {
                    Console.WriteLine($"Student {studentId} is not online");
                    return;
                }

                var message = status == "Approved"
                    ? $"Your booking request has been approved!"
                    : $"Your booking request has been rejected. Reason: {reason}";

                var notificationMessage = new
                {
                    Type = "BookingStatus",
                    Title = $"Booking {status}",
                    Message = message,
                    Status = status,
                    BookingDetails = new
                    {
                        EventId = booking.EventId,
                        Lab = booking.Lab.Name,
                        Zone = booking.Zone.Name,
                        Date = booking.StartTime.ToString("dd/MM/yyyy"),
                        StartTime = booking.StartTime.ToString("HH:mm"),
                        EndTime = booking.EndTime.ToString("HH:mm"),
                        Activity = booking.Title,
                        Status = status,
                        Reason = reason
                    },
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Client(connectionId)
                    .SendAsync("ReceiveStudentNotification", notificationMessage);

                Console.WriteLine($"Notification sent to student {studentId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
            }
        }
    }
}
