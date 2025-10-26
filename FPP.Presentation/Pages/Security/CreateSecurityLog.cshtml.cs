using System.Security.Claims;
using FPP.Application.DTOs.SecurityLog;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FPP.Presentation.Pages.Security
{
    [Authorize]
    public class CreateSecurityLogModel : PageModel
    {
        private readonly ISecurityLogService _securityLogService;
        private readonly IUserService _userService;

        public CreateSecurityLogModel(ISecurityLogService securityLogService, IUserService userService)
        {
            _securityLogService = securityLogService;
            _userService = userService;
        }
        [BindProperty]
        public SecurityLogRequest SecurityLogRequest { get; set; }

        [BindProperty(SupportsGet = true)]
        public int EventId { get; set; }

        public User CurrentUser { get; set; }

        public async Task<IActionResult> OnPost()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return NotFound();
            }

            SecurityLogRequest securityLog = new SecurityLogRequest()
            {
                Action = SecurityLogRequest.Action,
                EventId = EventId,
                Notes = SecurityLogRequest.Notes,
                PhotoUrl = SecurityLogRequest.PhotoUrl,
                SecurityId = userId
            };

            await _securityLogService.CreateSecurityLogAsync(securityLog);

            return RedirectToPage("/Security/SecurityLog", new { eventId = EventId });

        }

        public void OnGet()
        {
        }
    }
}
