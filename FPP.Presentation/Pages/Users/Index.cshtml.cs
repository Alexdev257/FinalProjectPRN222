using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FPP.Presentation.Pages.Users
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public List<User> Users { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Users = (await _userService.GetAll()).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var deleted = await _userService.Delete(id);
            if (deleted)
                TempData["StatusMessage"] = "User deleted successfully.";
            else
                TempData["ErrorMessage"] = "Failed to delete user.";

            return RedirectToPage();
        }
    }
}
