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

        // NEW: Security Log properties
        [BindProperty]
        public List<SecurityLog> SecurityLogs { get; set; } = new();
        [BindProperty]
        public int PendingSecurityLogsCount { get; set; }

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

            // NEW: Load security logs
            SecurityLogs = await _labEventService.GetAllSecurityLogsWithDetailsAsync();
            PendingSecurityLogsCount = SecurityLogs.Count(sl => sl.Status != "Acknowledged");

            return Page();
        }

        // Existing notification methods...
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

        // Existing booking methods...
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

        public async Task<IActionResult> OnGetBookingDetailsAsync(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out var uid)) return new JsonResult(new { success = false });

            var booking = await _labEventService.GetBookingWithDetailsAsync(eventId);
            if (booking == null) return new JsonResult(new { success = false, message = "Not found" });

            var logs = await _labEventService.GetSecurityLogsByEventIdAsync(eventId);
            var participants = await _labEventService.GetEventParticipantsAsync(eventId);

            return new JsonResult(new
            {
                success = true,
                booking = new
                {
                    eventId = booking.EventId,
                    title = booking.Title,
                    status = booking.Status,
                    startTime = booking.StartTime,
                    endTime = booking.EndTime,
                    lab = new { name = booking.Lab.Name },
                    zone = new { name = booking.Zone.Name },
                    organizer = new { name = booking.Organizer.Name, email = booking.Organizer.Email },
                    activityType = new { name = booking.ActivityType.Name }
                },
                securityLogs = logs.Select(l => new
                {
                    logId = l.LogId,
                    action = l.Action,
                    timestamp = l.Timestamp,
                    photoUrl = l.PhotoUrl,
                    notes = l.Notes,
                    security = new { name = l.Security.Name }
                }),
                participants = participants.Select(p => new
                {
                    user = new { name = p.User.Name, email = p.User.Email },
                    role = p.Role
                })
            });
        }

        // NEW: Get all security logs
        public async Task<IActionResult> OnGetSecurityLogsAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            try
            {
                var logs = await _labEventService.GetAllSecurityLogsWithDetailsAsync();

                var result = logs.Select(l => new
                {
                    logId = l.LogId,
                    action = l.Action,
                    timestamp = l.Timestamp,
                    photoUrl = l.PhotoUrl,
                    notes = l.Notes,
                    isAcknowledged = l.Status == "Acknowledged",
                    security = new { name = l.Security.Name },
                    eventDetails = new
                    {
                        eventId = l.Event.EventId,
                        title = l.Event.Title,
                        lab = l.Event.Lab?.Name,
                        zone = l.Event.Zone?.Name,
                        organizer = l.Event.Organizer?.Name,
                        startTime = l.Event.StartTime,
                        endTime = l.Event.EndTime
                    }
                }).ToList();

                return new JsonResult(new { success = true, logs = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching security logs: {ex.Message}");
                return new JsonResult(new { success = false, message = "Failed to load security logs" });
            }
        }

        // NEW: Get security log details
        public async Task<IActionResult> OnGetSecurityLogDetailsAsync(int logId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            try
            {
                var log = await _labEventService.GetSecurityLogByIdAsync(logId);
                if (log == null)
                    return new JsonResult(new { success = false, message = "Log not found" });

                var result = new
                {
                    logId = log.LogId,
                    action = log.Action,
                    timestamp = log.Timestamp,
                    photoUrl = log.PhotoUrl,
                    notes = log.Notes,
                    isAcknowledged = log.Status == "Acknowledged",
                    security = new
                    {
                        name = log.Security.Name,
                        email = log.Security.Email
                    },
                    eventDetails = new
                    {
                        eventId = log.Event.EventId,
                        title = log.Event.Title,
                        lab = log.Event.Lab?.Name,
                        zone = log.Event.Zone?.Name,
                        organizer = log.Event.Organizer?.Name,
                        startTime = log.Event.StartTime,
                        endTime = log.Event.EndTime,
                        status = log.Event.Status
                    }
                };

                return new JsonResult(new { success = true, log = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching security log details: {ex.Message}");
                return new JsonResult(new { success = false, message = "Failed to load log details" });
            }
        }

        // NEW: Acknowledge security log not yet
        public async Task<IActionResult> OnPostAcknowledgeSecurityLogAsync(int logId, string note)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            CurrentUser = await _userService.GetById(userId);
            if (CurrentUser == null || CurrentUser.Role != 2)
                return new JsonResult(new { success = false, message = "Unauthorized" });

            try
            {
                var success = await _labEventService.AcknowledgeSecurityLogAsync(logId, userId, note);

                if (success)
                {
                    // Send notification to security personnel
                    var log = await _labEventService.GetSecurityLogByIdAsync(logId);
                    if (log != null)
                    {
                        await SendNotificationToSecurity(log, note);
                    }

                    return new JsonResult(new { success = true });
                }

                return new JsonResult(new { success = false, message = "Failed to acknowledge log" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error acknowledging security log: {ex.Message}");
                return new JsonResult(new { success = false, message = "An error occurred" });
            }
        }

        // NEW: Send notification to security
        private async Task SendNotificationToSecurity(SecurityLog log, string managerNote)
        {
            try
            {
                var securityId = log.SecurityId;

                var message = $"Manager has acknowledged your security log for {log.Event.Lab.Name} - {log.Event.Zone.Name}. Note: {managerNote}";

                await _notificationService.CreateNotificationAsync(
                    securityId,
                    log.EventId,
                    message
                );

                var connectionId = NotificationBookingHub.GetConnectionId(securityId.ToString());
                if (!string.IsNullOrEmpty(connectionId))
                {
                    var notificationMessage = new
                    {
                        Type = "SecurityLogAcknowledged",
                        Title = "Log Acknowledged by Manager",
                        Message = message,
                        LogDetails = new
                        {
                            LogId = log.LogId,
                            Action = log.Action,
                            Timestamp = log.Timestamp,
                            ManagerNote = managerNote,
                            EventTitle = log.Event.Title
                        },
                        Timestamp = DateTime.Now
                    };

                    await _hubContext.Clients.Client(connectionId)
                        .SendAsync("ReceiveNotification", notificationMessage);

                    Console.WriteLine($"Notification sent to security {securityId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification to security: {ex.Message}");
            }
        }

        private async Task SendNotificationToStudent(LabEvent booking, string status, string? reason = null)
        {
            try
            {
                var studentId = booking.OrganizerId;
                var securities = await _userService.GetUsersByRoleAsync(3);
                var message = status == "Approved"
                    ? $"Your booking request for {booking.Lab.Name} - {booking.Zone.Name} on {booking.StartTime:dd/MM/yyyy} has been approved!"
                    : $"Your booking request for {booking.Lab.Name} - {booking.Zone.Name} on {booking.StartTime:dd/MM/yyyy} has been rejected. Reason: {reason ?? "No reason provided"}";

                await _notificationService.CreateNotificationAsync(
                    studentId,
                    booking.EventId,
                    message
                );

                var connectionId = NotificationBookingHub.GetConnectionId(studentId.ToString());
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

                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _hubContext.Clients.Client(connectionId)
                        .SendAsync("ReceiveStudentNotification", notificationMessage);

                    Console.WriteLine($"Notification sent to student {studentId}");
                }
                else
                {
                    Console.WriteLine($"Student {studentId} is offline, notification saved to DB");
                }
                foreach (var security in securities)
                {
                    var connectionIdOfSecurity = NotificationBookingHub.GetConnectionId(security.UserId.ToString());
                    if (!string.IsNullOrEmpty(connectionIdOfSecurity))
                    {
                        await _hubContext.Clients.Client(connectionIdOfSecurity)
                            .SendAsync("ReceiveSecurityNotification", notificationMessage);
                    }
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