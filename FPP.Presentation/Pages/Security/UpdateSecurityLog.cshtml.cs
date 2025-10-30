using FPP.Application.DTOs.SecurityLog;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FPP.Presentation.Pages.Security
{
    public class UpdateSecurityLogModel : PageModel
    {
        private readonly ISecurityLogService _securityLogService;
        private readonly IUserService _userService;

        public UpdateSecurityLogModel(ISecurityLogService securityLogService, IUserService userService)
        {
            _securityLogService = securityLogService;
            _userService = userService;
        }
        [BindProperty]
        public SecurityLogRequest SecurityLogRequest { get; set; }

        [BindProperty(SupportsGet = true)]
        public int EventId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int LogId { get; set; }
        public User CurrentUser { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var log = await _securityLogService.GetById(LogId);
            if (log == null)
            {
                return NotFound();
            }
            SecurityLogRequest = new SecurityLogRequest
            {
                SecurityId = log.SecurityId,
                EventId = log.EventId,
                Action = log.Action,
                Notes = log.Notes,
                PhotoUrl = log.PhotoUrl
            };

            EventId = log.EventId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return NotFound();
            }

            SecurityLogRequest.SecurityId = userId;
            SecurityLogRequest.EventId = EventId;

            await _securityLogService.UpdateSecurityLogAsync(LogId, SecurityLogRequest);
            return RedirectToPage("/Security/SecurityLog", new { eventId = EventId });

        }
    }
}
