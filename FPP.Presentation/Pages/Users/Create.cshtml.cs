using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FPP.Presentation.Pages.Users
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public User NewUser { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public CreateModel(IUserService userService)
        {
            _userService = userService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _userService.Add(NewUser);
            if (result)
                StatusMessage = "User added successfully!";
            else
                StatusMessage = "Failed to add user.";

            return RedirectToPage("Index");
        }
    }
}
