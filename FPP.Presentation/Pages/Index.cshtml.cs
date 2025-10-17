using FPP.Application.DTOs;
using FPP.Application.Interface.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FPP.Presentation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAuthService _authService;

        public IndexModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public LoginRequest request { get; set; } = default!;
        public string MessageLog { get; set; } = null!;

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) // ki?m tra các field trong model, n?u có l?i th? s? b?n l?i
            {
                return Page();
            }

            var rs = await _authService.Login(request);
            if (rs)
            {
                return RedirectToPage("Home");
            }
            else
            {
                MessageLog = "Not allowed!";
                return Page();
            }
        }
    }
}
