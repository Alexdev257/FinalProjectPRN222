using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using FPP.Infrastructure.Implements.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FPP.Presentation.Pages.Security
{
    [Authorize]
    public class SecurityDetailsModel : PageModel
    {
        public ISecurityLogService _securityLogService;

        public SecurityDetailsModel(ISecurityLogService securityLogService)
        {
            _securityLogService = securityLogService;
        }
        
        [BindProperty]
        public LabEvent LabEvent { get; set; } = new LabEvent();


        public async Task<IActionResult> OnGetAsync(int id)
        {
            LabEvent = await _securityLogService.GetLabEventByIdAsync(id);
            return Page();
        }
    }
}
