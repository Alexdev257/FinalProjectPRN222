using FPP.Application.DTOs.Lab;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FPP.Presentation.Pages
{
    public class HomeModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ILabService _labService;
        private readonly INotificationService _notificationService;
        private readonly IEventParticipantService _eventParticipantService;

        public HomeModel(
            IUserService userService,
            ILabService labService,
            INotificationService notificationService,
            IEventParticipantService eventParticipantService)
        {
            _userService = userService;
            _labService = labService;
            _notificationService = notificationService;
            _eventParticipantService = eventParticipantService;
        }

        public User CurrentUser { get; set; }

        [BindProperty]
        public int TotalLabCount { get; set; }

        [BindProperty]
        public int AvailableLabCount { get; set; }

        [BindProperty]
        public int MyUpcomingBookingsCount { get; set; }

        [BindProperty]
        public int UnreadNotificationCount { get; set; }

        [BindProperty]
        public List<LabVM> LabsList { get; set; } = new List<LabVM>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return RedirectToPage("/Login");
            }

            CurrentUser = await _userService.GetById(userId);
            if (CurrentUser == null)
            {
                return RedirectToPage("/Login");
            }

            TotalLabCount = _labService.GetTotalLabCountAsync();
            Console.WriteLine($"totalLab: {TotalLabCount}");

            AvailableLabCount = await _labService.GetAvailableLabCountAsync();
            Console.WriteLine($"AvailableLabCount: {AvailableLabCount}");

            UnreadNotificationCount = await _notificationService.CountUnreadNotificationsAsync(userId);
            Console.WriteLine($"UnreadNotificationCount: {UnreadNotificationCount}");

            MyUpcomingBookingsCount = await _eventParticipantService.CountUpcomingBookingsForUserAsync(userId);
            Console.WriteLine($"MyUpcomingBookingsCount: {MyUpcomingBookingsCount}");

            LabsList = await _labService.GetAllLabsWithAvailabilityAsync();
            Console.WriteLine($"LabsList: {LabsList.Count}");

            return Page();
        }

        // Get all notifications for current student
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
                        startTime = n.Event.StartTime,
                        endTime = n.Event.EndTime,
                        title = n.Event.Title,
                        status = n.Event.Status
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

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }
    }
}