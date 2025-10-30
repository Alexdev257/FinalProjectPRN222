using FPP.Application.DTOs.SecurityLog;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using FPP.Presentation.Hubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;

namespace FPP.Presentation.Pages
{
    [Authorize]
    public class SecurityDashboardModel : PageModel
    {
        private readonly ISecurityLogService _securityLogService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationBookingHub> _hubContext;

        public SecurityDashboardModel(
            ISecurityLogService securityLogService,
            IUserService userService,
            INotificationService notificationService,
            IHubContext<NotificationBookingHub> hubContext)
        {
            _securityLogService = securityLogService;
            _userService = userService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public User? CurrentUser { get; set; }

        [BindProperty]
        public List<LabEvent> LabEvents { get; set; } = new();

        [BindProperty]
        public List<SecurityLogResponse> SecurityLogs { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out var uid))
                return RedirectToPage("/Login");

            CurrentUser = await _userService.GetById(uid);
            if (CurrentUser?.Role != 3)
                return RedirectToPage("/Login");

            // Load all lab events
            LabEvents = (await _securityLogService.GetAllLabEventsAsync()).ToList();

            // Load security logs for current user
            SecurityLogs = (await _securityLogService.GetSecurityLogsBySecurityIdAsync(uid)).ToList();

            return Page();
        }

        public async Task<IActionResult> OnGetEventDetailsAsync(int id)
        {
            var evt = await _securityLogService.GetLabEventByIdAsync(id);
            if (evt == null)
                return Content("<p class='text-danger'>Event not found.</p>");

            var html = $@"
                <div class='detail-row'>
                    <div class='detail-label'>Title</div>
                    <div class='detail-value'><strong>{evt.Title}</strong></div>
                </div>
                <div class='detail-row'>
                    <div class='detail-label'>Lab</div>
                    <div class='detail-value'>{evt.Lab?.Name ?? "—"}</div>
                </div>
                <div class='detail-row'>
                    <div class='detail-label'>Zone</div>
                    <div class='detail-value'>{evt.Zone?.Name ?? "—"}</div>
                </div>
                <div class='detail-row'>
                    <div class='detail-label'>Date</div>
                    <div class='detail-value'>{evt.StartTime:dd/MM/yyyy}</div>
                </div>
                <div class='detail-row'>
                    <div class='detail-label'>Time</div>
                    <div class='detail-value'>{evt.StartTime:HH:mm} ? {evt.EndTime:HH:mm}</div>
                </div>
                <div class='detail-row'>
                    <div class='detail-label'>Organizer</div>
                    <div class='detail-value'>{evt.Organizer?.Name ?? "—"}</div>
                </div>
                <div class='detail-row'>
                    <div class='detail-label'>Status</div>
                    <div class='detail-value'><span class='status-badge status-{evt.Status.ToLower()}'>{evt.Status}</span></div>
                </div>
                <div class='detail-row'>
                    <div class='detail-label'>Description</div>
                    <div class='detail-value'>{evt.Description ?? "No description provided"}</div>
                </div>";

            return Content(html, "text/html");
        }

        public async Task<JsonResult> OnPostCreateLogAsync()
        {
            try
            {
                // Get form data
                var eventId = int.Parse(Request.Form["EventId"]);
                var action = Request.Form["Action"].ToString();
                var notes = Request.Form["Notes"].ToString();
                var photoUrl = Request.Form["PhotoUrl"].ToString();

                // Validate required fields
                if (eventId <= 0 || string.IsNullOrWhiteSpace(action))
                {
                    return new JsonResult(new { success = false, message = "Event and Action are required" });
                }

                // Get current user ID
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim?.Value, out var secId))
                {
                    return new JsonResult(new { success = false, message = "Unauthorized access" });
                }

                // Verify user is security
                var user = await _userService.GetById(secId);
                if (user?.Role != 3)
                {
                    return new JsonResult(new { success = false, message = "Only security officers can create logs" });
                }

                // Create security log request
                var request = new SecurityLogRequest
                {
                    EventId = eventId,
                    SecurityId = secId,
                    Action = action,
                    Notes = notes,
                    PhotoUrl = photoUrl
                };

                // Create security log
                await _securityLogService.CreateSecurityLogAsync(request);

                // Get event details for notification
                var evt = await _securityLogService.GetLabEventByIdAsync(eventId);

                if (evt != null)
                {
                    var msg = $"Security logged: {action} at {evt.Lab?.Name} - {evt.Title}";

                    var notificationMessage = new
                    {
                        Title = "New Security Log",
                        Message = msg,
                        Timestamp = DateTime.Now,
                        EventDetails = new
                        {
                            EventId = evt.EventId,
                            Lab = evt.Lab?.Name,
                            Zone = evt.Zone?.Name,
                            Action = action
                        }
                    };

                    var managers = await _userService.GetUsersByRoleAsync(2);

                    foreach (var manager in managers)
                    {
                        var connectionId = NotificationBookingHub.GetConnectionId(manager.UserId.ToString());
                        if (!string.IsNullOrEmpty(connectionId))
                        {
                            await _hubContext.Clients.Client(connectionId)
                                .SendAsync("ReceiveSecurityLogNotification", notificationMessage);

                            Console.WriteLine($"Sent security log notif to Manager {manager.UserId}");
                        }
                    }
                    //await _hubContext.Clients.Group("Managers").SendAsync("ReceiveSecurityLogNotification", new
                    //{
                    //    Title = "New Security Log",
                    //    Message = msg,
                    //    Timestamp = DateTime.Now,
                    //    EventDetails = new
                    //    {
                    //        EventId = evt.EventId,
                    //        Lab = evt.Lab?.Name,
                    //        Zone = evt.Zone?.Name,
                    //        Action = action
                    //    }
                    //});
                }

                return new JsonResult(new { success = true, message = "Security log created successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating security log: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new JsonResult(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public async Task<JsonResult> OnGetNotificationsAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userId, out var uid))
                {
                    return new JsonResult(new { success = false, message = "Unauthorized" });
                }

                var notifs = await _notificationService.GetNotificationsByUserIdAsync(uid);

                var result = notifs.Select(n => new
                {
                    id = n.NotificationId,
                    message = n.Message,
                    sentAt = n.SentAt,
                    isRead = n.IsRead
                }).OrderByDescending(n => n.sentAt).ToList();

                return new JsonResult(new { success = true, notifications = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading notifications: {ex.Message}");
                return new JsonResult(new { success = false, message = "Failed to load notifications" });
            }
        }

        public async Task<IActionResult> OnPostMarkReadAsync(int id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking notification as read: {ex.Message}");
                return new JsonResult(new { success = false, message = "Failed to mark notification as read" });
            }
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }
    }
}