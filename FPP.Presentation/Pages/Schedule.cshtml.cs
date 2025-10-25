using FPP.Application.DTOs.LabEvent;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FPP.Presentation.Pages
{
    [Authorize]
    public class ScheduleModel : PageModel
    {
        // Inject only the required Services
        private readonly ILabService _labService;
        private readonly IUserService _userService;
        private readonly IEventParticipantService _eventParticipantService; // Added

        public ScheduleModel(
            ILabService labService,
            IUserService userService,
            IEventParticipantService eventParticipantService) // Added
        {
            _labService = labService;
            _userService = userService;
            _eventParticipantService = eventParticipantService; // Added
        }

        // --- Properties remain the same ---
        public User CurrentUser { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Today.Year;

        [BindProperty(SupportsGet = true)]
        public int Month { get; set; } = DateTime.Today.Month;

        public DateTime FirstDayOfMonth { get; private set; }
        public DateTime LastDayOfMonth { get; private set; }
        public DayOfWeek FirstDayOfWeekOfMonth { get; private set; }
        public int DaysInMonth { get; private set; }

        public Dictionary<int, List<BookingCalendarItem>> UserBookingsForMonth { get; set; } = new Dictionary<int, List<BookingCalendarItem>>();

        // ViewModel remains the same
        //public class BookingCalendarItem
        //{
        //    public int EventId { get; set; }
        //    public string LabName { get; set; } = string.Empty;
        //    public DateTime StartTime { get; set; }
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            // --- L?y User Info (using UserService) ---
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return RedirectToPage("/Login");
            }
            CurrentUser = await _userService.GetById(userId);
            if (CurrentUser == null) return RedirectToPage("/Login");

            // --- Tính toán thông tin tháng (logic remains the same) ---
            try
            {
                FirstDayOfMonth = new DateTime(Year, Month, 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                Year = DateTime.Today.Year;
                Month = DateTime.Today.Month;
                FirstDayOfMonth = new DateTime(Year, Month, 1);
            }
            DaysInMonth = DateTime.DaysInMonth(Year, Month);
            LastDayOfMonth = FirstDayOfMonth.AddMonths(1).AddDays(-1);
            FirstDayOfWeekOfMonth = FirstDayOfMonth.DayOfWeek;

            // --- L?y bookings c?a user (using EventParticipantService) ---
            UserBookingsForMonth = await _eventParticipantService.GetUserBookingsGroupedByDayAsync(userId, Year, Month); // Call the service method

            // Note: Filter dropdown labs (AvailableLabs) is removed as it's not used in the calendar view
            // If you need it for something else, inject and call ILabService.GetAllLabsAsync() here.

            return Page();
        }

        // Helpers remain the same
        public DateTime GetPreviousMonth() => FirstDayOfMonth.AddMonths(-1);
        public DateTime GetNextMonth() => FirstDayOfMonth.AddMonths(1);
    }
}
