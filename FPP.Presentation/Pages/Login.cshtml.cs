using FPP.Application.DTOs.Auth;
using FPP.Application.Interface.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FPP.Presentation.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;
        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public LoginRequest LoginInput { get; set; }

        [BindProperty]
        public RegisterRequest RegisterInput { get; set; }

        [BindProperty]
        public ForgotPasswordRequest ForgotPasswordInput { get; set; }

        [BindProperty]
        public string OtpInput { get; set; }

        [TempData]
        public string Message { get; set; }

        [TempData]
        public string MessageType { get; set; } // success, error

        [TempData]
        public string CurrentEmail { get; set; } // L?u email ?? hi?n th? trong OTP form

        [TempData]
        public string OtpType { get; set; } // "register" ho?c "forgotpassword"

        public void OnGet()
        {
            // Trang load l?n ??u
        }

        public async Task<IActionResult> OnPostCreateManagerAsync()
        {
            var user = new FPP.Domain.Entities.User
            {
                CreatedAt = DateTime.Now,
                Email = "adminfpt@fpt.edu.vn",
                Name = "Admin FPT",
                PasswordHash = "Admin123@",
                Role = 2,
            };
            var result = await _authService.CreateManagerAccount(user);
            if (result)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("/");    
            }
        }

        // === LOGIN ===
        public async Task<IActionResult> OnPostLoginAsync()
        {
            // B? qua validation ban ??u vì có th? b? conflict v?i các field khác
            ModelState.Clear();

            if (string.IsNullOrEmpty(LoginInput?.Email) || string.IsNullOrEmpty(LoginInput?.Password))
            {
                Message = "Please fill in all fields correctly.";
                MessageType = "error";
                return Page();
            }

            var result = await _authService.Login(LoginInput);

            if (result == null)
            {
                Message = "Invalid email or password!";
                MessageType = "error";
                return Page();
                
            }
            var claims = new List<Claim>
                {

                    new Claim(ClaimTypes.Email, result.Email),
                    new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                    new Claim(ClaimTypes.Role, result.Role.ToString()),
                };

            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(claimIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                }
                );
            if (result.Role == 1)
            {
                return RedirectToPage("/Home");
            }
            else if (result.Role == 2)
            {
                return RedirectToPage("/ManagerDashboard");
            }
            else if (result.Role == 3)
            {
                return RedirectToPage("/SecurityHome");
            }
            else
            {
                return Page();
            }
        }

        // === REGISTER ===
        public async Task<IActionResult> OnPostRegisterAsync()
        {
            ModelState.Clear();

            if (string.IsNullOrEmpty(RegisterInput?.Name) ||
                string.IsNullOrEmpty(RegisterInput?.Email) ||
                string.IsNullOrEmpty(RegisterInput?.Password))
            {
                Message = "Please fill in all fields correctly.";
                MessageType = "error";
                return Page();
            }

            var result = await _authService.Register(RegisterInput);

            if (result)
            {
                // G?i OTP thành công
                CurrentEmail = RegisterInput.Email;
                OtpType = "register";
                Message = "OTP has been sent to your email!";
                MessageType = "success";
                return Page(); // S? m? form OTP b?ng JavaScript
            }
            else
            {
                Message = "Email already exists or failed to send OTP!";
                MessageType = "error";
                return Page();
            }
        }

        // === VERIFY REGISTER OTP ===
        public async Task<IActionResult> OnPostVerifyRegisterAsync()
        {
            ModelState.Clear();

            if (string.IsNullOrEmpty(OtpInput))
            {
                Message = "Please enter OTP code!";
                MessageType = "error";
                OtpType = "register";
                return Page();
            }

            var request = new VerifyRegisterRequest
            {
                OTP = OtpInput
            };

            var result = await _authService.VerifyRegsiter(request);

            if (result)
            {
                Message = "Registration successful! Please login.";
                MessageType = "success";
                OtpType = null;
                return Page(); // S? m? form login b?ng JavaScript
            }
            else
            {
                Message = "Invalid or expired OTP!";
                MessageType = "error";
                OtpType = "register";
                return Page();
            }
        }

        // === FORGOT PASSWORD ===
        public async Task<IActionResult> OnPostForgotPasswordAsync()
        {
            ModelState.Clear();

            if (string.IsNullOrEmpty(ForgotPasswordInput?.Email) ||
                string.IsNullOrEmpty(ForgotPasswordInput?.NewPassword))
            {
                Message = "Please fill in all fields correctly.";
                MessageType = "error";
                return Page();
            }

            var result = await _authService.ForgotPassword(ForgotPasswordInput);

            if (result)
            {
                // G?i OTP thành công
                CurrentEmail = ForgotPasswordInput.Email;
                OtpType = "forgotpassword";
                Message = "OTP has been sent to your email!";
                MessageType = "success";
                return Page(); // S? m? form OTP b?ng JavaScript
            }
            else
            {
                Message = "Email not found or failed to send OTP!";
                MessageType = "error";
                return Page();
            }
        }

        // === VERIFY FORGOT PASSWORD OTP ===
        public async Task<IActionResult> OnPostVerifyForgotPasswordAsync()
        {
            ModelState.Clear();

            if (string.IsNullOrEmpty(OtpInput))
            {
                Message = "Please enter OTP code!";
                MessageType = "error";
                OtpType = "forgotpassword";
                return Page();
            }

            var request = new VerifyForgotPasswordRequest
            {
                OTP = OtpInput
            };

            var result = await _authService.VerifyForgotPassword(request);

            if (result)
            {
                Message = "Password reset successful! Please login with new password.";
                MessageType = "success";
                OtpType = null;
                return Page(); // S? m? form login b?ng JavaScript
            }
            else
            {
                Message = "Invalid or expired OTP!";
                MessageType = "error";
                OtpType = "forgotpassword";
                return Page();
            }
        }
    }
}
