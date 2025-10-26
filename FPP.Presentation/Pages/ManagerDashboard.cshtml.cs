using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using FPP.Presentation.Hubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationBookingHub> _hubContext;

        public ManagerDashboardModel(
            ILabEventService labEventService,
            IUserService userService,
            INotificationService notificationService,
            IHubContext<NotificationBookingHub> hubContext)
        {
            _labEventService = labEventService;
            _userService = userService;
            _notificationService = notificationService;
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
            if (CurrentUser == null || CurrentUser.Role != 2)
                return RedirectToPage("/Login");

            Bookings = await _labEventService.GetAllBookingsWithDetailsAsync();

            var today = DateTime.Today;
            PendingCount = Bookings.Count(b => b.Status == "Pending");
            ApprovedCount = Bookings.Count(b => b.Status == "Approved" && b.CreatedAt.Date == today);
            RejectedCount = Bookings.Count(b => b.Status == "Rejected" && b.CreatedAt.Date == today);
            TotalCount = Bookings.Count;

            return Page();
        }

        // Get all notifications for current user
        public async Task<IActionResult> OnGetNotificationsAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            try
            {
                var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);

                var result = notifications.Select(n => new
                {
                    notificationId = n.NotificationId,
                    message = n.Message,
                    sentAt = n.SentAt,
                    isRead = n.IsRead,
                    eventDetails = new
                    {
                        eventId = n.Event.EventId,
                        lab = n.Event.Lab?.Name,
                        zone = n.Event.Zone?.Name,
                        organizer = n.Event.Organizer?.Name,
                        startTime = n.Event.StartTime,
                        endTime = n.Event.EndTime,
                        title = n.Event.Title
                    }
                }).ToList();

                return new JsonResult(new { success = true, notifications = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications: {ex.Message}");
                return new JsonResult(new { success = false, message = "Failed to load notifications" });
            }
        }

        // Get unread count
        public async Task<IActionResult> OnGetUnreadCountAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            try
            {
                var count = await _notificationService.GetUnreadCountAsync(userId);
                return new JsonResult(new { success = true, count });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting unread count: {ex.Message}");
                return new JsonResult(new { success = false, count = 0 });
            }
        }

        // Mark notification as read
        public async Task<IActionResult> OnPostMarkAsReadAsync(int notificationId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            try
            {
                var success = await _notificationService.MarkAsReadAsync(notificationId);
                return new JsonResult(new { success });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking as read: {ex.Message}");
                return new JsonResult(new { success = false });
            }
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
                var booking = await _labEventService.GetBookingByIdAsync(eventId);
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
                var booking = await _labEventService.GetBookingByIdAsync(eventId);
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

                var message = status == "Approved"
                    ? $"Your booking request for {booking.Lab.Name} - {booking.Zone.Name} on {booking.StartTime:dd/MM/yyyy} has been approved!"
                    : $"Your booking request for {booking.Lab.Name} - {booking.Zone.Name} on {booking.StartTime:dd/MM/yyyy} has been rejected. Reason: {reason ?? "No reason provided"}";

                await _notificationService.CreateNotificationAsync(
                    studentId,
                    booking.EventId,
                    message
                );

                var connectionId = NotificationBookingHub.GetConnectionId(studentId.ToString());
                if (!string.IsNullOrEmpty(connectionId))
                {
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
                else
                {
                    Console.WriteLine($"Student {studentId} is offline, notification saved to DB");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage("/Login");
        }
    }
}