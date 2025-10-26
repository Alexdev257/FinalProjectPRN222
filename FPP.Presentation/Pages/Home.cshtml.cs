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

        // --- Properties gi? nguyên ---
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

        // ViewModel gi? nguyên
        //public class LabViewModel
        //{
        //    public int LabId { get; set; }
        //    public string Name { get; set; } = string.Empty;
        //    public string? Description { get; set; }
        //    public string? Location { get; set; }
        //    public bool IsAvailableNow { get; set; }
        //}


        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return RedirectToPage("/Login");
            }

            CurrentUser = await _userService.GetById(userId); // G?i service
            if (CurrentUser == null)
            {
                return RedirectToPage("/Login");
            }


            // --- 2. L?y Stats (dùng các services t??ng ?ng) ---
            TotalLabCount = _labService.GetTotalLabCountAsync();
            Console.WriteLine($"totalLab: {TotalLabCount}");
            AvailableLabCount = await _labService.GetAvailableLabCountAsync();
            Console.WriteLine($"AvailableLabCount: {AvailableLabCount}");
            UnreadNotificationCount = await _notificationService.CountUnreadNotificationsAsync(userId);
            Console.WriteLine($"UnreadNotificationCount: {UnreadNotificationCount}");
            MyUpcomingBookingsCount = await _eventParticipantService.CountUpcomingBookingsForUserAsync(userId);
            Console.WriteLine($"MyUpcomingBookingsCount: {MyUpcomingBookingsCount}");


            // --- 3. L?y danh sách Labs (dùng LabService) ---
            LabsList = await _labService.GetAllLabsWithAvailabilityAsync();
            Console.WriteLine($"LabsList: {LabsList.Count}");


            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage("/Login");
        }
    }
}
