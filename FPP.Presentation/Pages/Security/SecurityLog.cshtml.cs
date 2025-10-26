using FPP.Application.DTOs.SecurityLog;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FPP.Presentation.Pages.Security
{
    [Authorize]
    public class SecurityLogModel : PageModel
    {
        public ISecurityLogService _securityLogService;

        private const int PageSize = 10;

        public SecurityLogModel(ISecurityLogService securityLogService)
        {
            _securityLogService = securityLogService;
        }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int EventId { get; set; }


        [BindProperty]
        public List<SecurityLogResponse> SecurityLogResponses { get; set; } = new List<SecurityLogResponse>();

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            PageIndex = pageIndex < 1 ? 1 : pageIndex;
            if (EventId < 0)
            {
               return Page();
            }

            SecurityLogResponses = (await _securityLogService
                .GetAllSecurityLogsAsync(PageIndex, PageSize, EventId))
                .ToList();

            return Page();
        }
    }
}
