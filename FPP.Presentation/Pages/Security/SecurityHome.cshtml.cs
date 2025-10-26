using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FPP.Presentation.Pages.Security
{
    [Authorize]
    public class SecurityHomeModel : PageModel
    {
        public ILabEventService _labEventService;
        public ISecurityLogService _securityLogService;

        [BindProperty]
        public List<LabEvent> LabEvents { get; set; } = new List<LabEvent>();

        public SecurityHomeModel(ILabEventService labEventService, ISecurityLogService securityLogService)
        {
            _labEventService = labEventService;
            _securityLogService = securityLogService;
        }

        public async Task OnGetAsync()
        {
            LabEvents = (await _securityLogService.GetAllLabEventsAsync()).ToList();
        }
    }
}
