using FPP.Application.DTOs;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FPP.Presentation.Pages.Booking
{
    [Authorize]
    public class CreateBookingModel : PageModel
    {
        // Inject Services needed
        private readonly ILabService _labService;
        private readonly IActivityTypeService _activityTypeService;
        private readonly IUserService _userService;
        private readonly ILabEventService _labEventService; // Added

        public CreateBookingModel(
            ILabService labService,
            IActivityTypeService activityTypeService,
            IUserService userService,
            ILabEventService labEventService) // Added
        {
            _labService = labService;
            _activityTypeService = activityTypeService;
            _userService = userService;
            _labEventService = labEventService; // Added
        }

        [BindProperty]
        public BookingInputModel Input { get; set; } = new BookingInputModel();

        // --- Other properties remain the same ---
        public User CurrentUser { get; set; }
        public SelectList LabOptions { get; set; }
        public SelectList ActivityTypeOptions { get; set; }

        // --- InputModel remains the same ---

        // --- OnGetAsync remains largely the same (calls services) ---
        public async Task<IActionResult> OnGetAsync(int? labId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId)) return RedirectToPage("/Login");
            CurrentUser = await _userService.GetById(userId);
            if (CurrentUser == null) return RedirectToPage("/Login");

            var labs = await _labService.GetAllLabsAsync();
            var activityTypes = await _activityTypeService.GetAllActivityTypesAsync();

            LabOptions = new SelectList(labs, nameof(Lab.LabId), nameof(Lab.Name), labId);
            ActivityTypeOptions = new SelectList(activityTypes, nameof(ActivityType.ActivityTypeId), nameof(ActivityType.Name));

            Input.BookingDate = DateTime.Today;

            if (labId.HasValue)
            {
                Input.LabId = labId.Value;
            }

            return Page();
        }

        // --- OnGetZonesAsync remains the same (calls LabService) ---
        public async Task<JsonResult> OnGetZonesAsync(int labId)
        {
            var zones = await _labService.GetZonesByLabIdAsync(labId);
            var result = zones.Select(z => new { id = z.ZoneId, name = z.Name }).ToList();
            return new JsonResult(result);
        }

        // --- OnPostAsync uses LabEventService ---
        public async Task<IActionResult> OnPostAsync()
        {
            // --- Reload necessary data (remains the same) ---
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId)) return RedirectToPage("/Login");
            CurrentUser = await _userService.GetById(userId);
            if (CurrentUser == null) return RedirectToPage("/Login");

            // --- Basic Validation & Date/Time Combination (remains the same) ---
            // Combine Date and Time
            DateTime startDateTime;
            DateTime endDateTime;
            try
            {
                startDateTime = Input.BookingDate.Date + Input.StartTime;
                endDateTime = Input.BookingDate.Date + Input.EndTime;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ModelState.AddModelError("", "Invalid date/time combination.");
                // Reload dropdowns before returning Page
                await ReloadDropdownsAsync();
                return Page();
            }

            if (startDateTime >= endDateTime)
            {
                ModelState.AddModelError("Input.EndTime", "End time must be after start time.");
            }
            if (startDateTime < DateTime.Now)
            {
                ModelState.AddModelError("Input.StartTime", "Booking must be for a future time.");
            }
            // Add other basic validations...

            // --- Check for Time Conflicts using Service ---
            if (ModelState.IsValid) // Only check conflict if basic validation passes
            {
                bool hasConflict = await _labEventService.CheckTimeConflictAsync(Input.LabId, Input.ZoneId, startDateTime, endDateTime);
                if (hasConflict)
                {
                    ModelState.AddModelError("", $"The selected time slot in this zone is already booked or pending approval.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Reload dropdowns before returning Page with errors
                await ReloadDropdownsAsync();
                return Page();
            }

            // --- Create Booking using Service ---
            var (success, errorMessage) = await _labEventService.CreateBookingAsync(Input, userId);

            if (success)
            {
                TempData["StatusMessage"] = "Booking request submitted successfully! It is pending approval.";
                return RedirectToPage("/MyBookings");
            }
            else
            {
                // Add the error message from the service to ModelState
                ModelState.AddModelError("", errorMessage);
                // Reload dropdowns before returning Page with the error
                await ReloadDropdownsAsync();
                return Page();
            }
        }

        // Helper method to reload dropdowns
        private async Task ReloadDropdownsAsync()
        {
            var labs = await _labService.GetAllLabsAsync();
            var activityTypes = await _activityTypeService.GetAllActivityTypesAsync();
            LabOptions = new SelectList(labs, nameof(Lab.LabId), nameof(Lab.Name), Input.LabId);
            ActivityTypeOptions = new SelectList(activityTypes, nameof(ActivityType.ActivityTypeId), nameof(ActivityType.Name), Input.ActivityTypeId);
            // Optionally reload zones if LabId is selected
            if (Input.LabId > 0)
            {
                // You might want to store zones in a separate property or handle this in JS on error
            }
        }
    }
}
