using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using FPP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FPP.Presentation.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public User EditUser { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public EditModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
                return NotFound();

            EditUser = user;
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _userService.Update(EditUser);
            if (result)
                StatusMessage = "User updated successfully!";
            else
                StatusMessage = "Failed to update user.";

            return RedirectToPage("Index");
        }
    }
}
