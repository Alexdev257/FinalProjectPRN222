using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using FPP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FPP.Presentation.Pages.Users
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly IUserService _userService;

        public DeleteModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User User { get; set; }

        // Hiển thị thông tin user trước khi xác nhận xóa
        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await _userService.FindAsync(id);
            if (User == null)
                return NotFound();

            return Page();
        }

        // Thực hiện xóa sau khi bấm nút Confirm
        public async Task<IActionResult> OnPostAsync()
        {
            var existingUser = await _userService.FindAsync(User.UserId);
            if (existingUser == null)
                return NotFound();

            await _userService.Delete(existingUser.UserId);

            TempData["Message"] = "User deleted successfully!";
            return RedirectToPage("./Index");
        }
    }

}
