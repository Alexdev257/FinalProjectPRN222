using FPP.Application.DTOs.Auth;
using FPP.Application.Interface.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FPP.Presentation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAuthService _authService;

        public IndexModel(IAuthService authService)
        {
            _authService = authService;
        }

        //[BindProperty]
        //public LoginRequest LoginRequest { get; set; }
        //[BindProperty]
        //public RegisterRequest RegisterRequest { get; set; }
        //[BindProperty]
        //public VerifyRegisterRequest VerifyRegisterRequest { get; set; }
        //[BindProperty]
        //public ForgotPasswordRequest ForgotPasswordRequest { get; set; }
        //[BindProperty]
        //public VerifyForgotPasswordRequest VerifyForgotPasswordRequest { get; set; }
        //[TempData]
        //public string RegisterErrorMessage { get; set; }
        //[TempData]
        //public string LoginErrorMessage { get; set; }
        //[TempData]
        //public string ForgotPasswordErrorMessage { get; set; }
        //[TempData]
        //public string VerifyRegisterErrorMessage { get; set; }
        //[TempData]
        //public string VerifyForgotPasswordErrorMessage { get; set; }
        //public void OnGet()
        //{
        //}
        //public async Task<IActionResult> OnPostLoginAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }
        //    try
        //    {
        //        var rs = await _authService.Login(LoginRequest);
        //        if (rs)
        //        {
        //            return RedirectToPage("/Home");
        //        }
        //        else
        //        {
        //            LoginErrorMessage = "Invalid email or password.";
        //            return Page();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LoginErrorMessage = "Something went wrong. Please try again later.";
        //        return Page();
        //    }
        //}
        //public async Task<IActionResult> OnPostRegisterAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }
        //    try
        //    {
        //        var rs = await _authService.Register(RegisterRequest);
        //        if (rs)
        //        {
        //            return RedirectToPage("/Index");
        //        }
        //        else
        //        {
        //            RegisterErrorMessage = "Email already exists.";
        //            return Page();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RegisterErrorMessage = "Something went wrong. Please try again later.";
        //        return Page();
        //    }
        //}
        //public async Task<IActionResult> OnVerifyRegisterAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }
        //    try
        //    {
        //        var rs = await _authService.VerifyRegsiter(VerifyRegisterRequest);
        //        if (rs)
        //        {
        //            return RedirectToPage("/Index");
        //        }
        //        else
        //        {
        //            VerifyRegisterErrorMessage = "Invalid OTP code.";
        //            return Page();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        VerifyRegisterErrorMessage = "Something went wrong. Please try again later.";
        //        return Page();
        //    }
        //}
        //public async Task<IActionResult> OnPostForgotPasswordAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }
        //    try
        //    {
        //        var rs = await _authService.ForgotPassword(ForgotPasswordRequest);
        //        if (rs)
        //        {
        //            return RedirectToPage("/Index");
        //        }
        //        else
        //        {
        //            ForgotPasswordErrorMessage = "Email does not exist.";
        //            return Page();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ForgotPasswordErrorMessage = "Something went wrong. Please try again later.";
        //        return Page();
        //    }
        //}
        //public async Task<IActionResult> OnPostVerifyForgotPasswordAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }
        //    try
        //    {
        //        var rs = await _authService.VerifyForgotPassword(VerifyForgotPasswordRequest);
        //        if (rs)
        //        {
        //            return RedirectToPage("/Index");
        //        }
        //        else
        //        {
        //            VerifyForgotPasswordErrorMessage = "Invalid OTP code.";
        //            return Page();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        VerifyForgotPasswordErrorMessage = "Something went wrong. Please try again later.";
        //        return Page();
        //    }
        //}

        // --- Bind Properties (Initialize to prevent null issues) ---
        [BindProperty] public LoginRequest LoginRequest { get; set; } = new();
        [BindProperty] public RegisterRequest RegisterRequest { get; set; } = new();
        [BindProperty] public VerifyRegisterRequest VerifyRegisterRequest { get; set; } = new();
        [BindProperty] public ForgotPasswordRequest ForgotPasswordRequest { get; set; } = new();
        [BindProperty] public VerifyForgotPasswordRequest VerifyForgotPasswordRequest { get; set; } = new();

        // --- TempData for SUCCESS messages after redirects ---
        [TempData] public string LoginSuccessMessage { get; set; }
        [TempData] public string GeneralSuccessMessage { get; set; }

        // --- TempData for ERROR messages (fallback) ---
        [TempData] public string RegisterErrorMessage { get; set; }
        [TempData] public string LoginErrorMessage { get; set; }
        [TempData] public string ForgotPasswordErrorMessage { get; set; }
        [TempData] public string VerifyRegisterErrorMessage { get; set; }
        [TempData] public string VerifyForgotPasswordErrorMessage { get; set; }

        public void OnGet() { }

        // --- Helper to extract errors for a specific model ---
        private string GetModelErrors(string modelPrefix)
        {
            return string.Join(" ", ModelState
                .Where(kvp => kvp.Key.StartsWith(modelPrefix + ".") || kvp.Key == modelPrefix) // Filter keys by prefix
                .SelectMany(kvp => kvp.Value.Errors)
                .Select(e => e.ErrorMessage));
        }


        // --- AJAX Handlers ---

        public async Task<IActionResult> OnPostLoginAsync()
        {
            // Explicitly validate ONLY LoginRequest
            if (!TryValidateModel(LoginRequest, nameof(LoginRequest)))
            {
                return new JsonResult(new { success = false, message = GetModelErrors(nameof(LoginRequest)) });
            }
            try
            {
                // *** IMPORTANT: Ensure your AuthService.Login uses password verification (like BcryptHelper.Verify), NOT direct comparison ***
                var rs = await _authService.Login(LoginRequest);
                if (rs)
                {
                    // TODO: Implement actual sign-in logic (e.g., HttpContext.SignInAsync)
                    LoginSuccessMessage = "??ng nh?p thành công!";
                    return new JsonResult(new { success = true, redirectUrl = Url.Page("/Home") }); // Redirect URL for JS
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Email ho?c m?t kh?u không chính xác." });
                }
            }
            catch (Exception ex)
            {
                // TODO: Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i h? th?ng khi ??ng nh?p." });
            }
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            // Explicitly validate ONLY RegisterRequest
            if (!TryValidateModel(RegisterRequest, nameof(RegisterRequest)))
            {
                return new JsonResult(new { success = false, message = GetModelErrors(nameof(RegisterRequest)) });
            }
            try
            {
                var rs = await _authService.Register(RegisterRequest);
                if (rs)
                {
                    return new JsonResult(new { success = true, email = RegisterRequest.Email });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Email này ?ã t?n t?i ho?c không th? g?i email xác th?c." });
                }
            }
            catch (Exception ex)
            {
                // TODO: Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i h? th?ng khi ??ng ký." });
            }
        }

        public async Task<IActionResult> OnPostVerifyRegisterAsync()
        {
            // Manually set OTP from hidden input before validation if needed,
            // or ensure the hidden input binding works correctly.
            // We will rely on JS putting value into the hidden asp-for input.

            // Explicitly validate ONLY VerifyRegisterRequest
            if (!TryValidateModel(VerifyRegisterRequest, nameof(VerifyRegisterRequest)))
            {
                // Also manually check combined OTP input if necessary
                if (string.IsNullOrEmpty(VerifyRegisterRequest.OTP) || VerifyRegisterRequest.OTP.Length != 6 || !VerifyRegisterRequest.OTP.All(char.IsDigit))
                {
                    return new JsonResult(new { success = false, message = "OTP ph?i có 6 ch? s?." });
                }
                return new JsonResult(new { success = false, message = GetModelErrors(nameof(VerifyRegisterRequest)) });
            }

            try
            {
                var rs = await _authService.VerifyRegsiter(VerifyRegisterRequest);
                if (rs)
                {
                    GeneralSuccessMessage = "Xác th?c tài kho?n thành công! Vui lòng ??ng nh?p.";
                    return new JsonResult(new { success = true, message = "Xác th?c thành công! Vui lòng ??ng nh?p." });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Mã OTP không h?p l? ho?c ?ã h?t h?n." });
                }
            }
            catch (Exception ex)
            {
                // TODO: Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i khi xác th?c OTP." });
            }
        }

        public async Task<IActionResult> OnPostForgotPasswordAsync()
        {
            // Explicitly validate ONLY ForgotPasswordRequest
            if (!TryValidateModel(ForgotPasswordRequest, nameof(ForgotPasswordRequest)))
            {
                // Your ForgotPasswordRequest DTO only has Email, so this check is simpler.
                // If other properties were present, you'd filter errors like GetModelErrors.
                var emailError = ModelState[nameof(ForgotPasswordRequest) + ".Email"]?.Errors.FirstOrDefault()?.ErrorMessage;
                return new JsonResult(new { success = false, message = emailError ?? "Email không h?p l?." });

            }
            try
            {
                // Pass only the necessary data (Email) if your service expects that.
                // Assuming your service was updated to accept the simplified DTO:
                var rs = await _authService.ForgotPassword(ForgotPasswordRequest);
                if (rs)
                {
                    return new JsonResult(new { success = true, email = ForgotPasswordRequest.Email });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Email không t?n t?i trong h? th?ng ho?c không th? g?i email." });
                }
            }
            catch (Exception ex)
            {
                // TODO: Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i h? th?ng khi g?i yêu c?u." });
            }
        }

        public async Task<IActionResult> OnPostVerifyForgotPasswordAsync()
        {
            // Manually set OTP from hidden input before validation if needed.
            // We will rely on JS putting value into the hidden asp-for input.

            // Explicitly validate ONLY VerifyForgotPasswordRequest
            if (!TryValidateModel(VerifyForgotPasswordRequest, nameof(VerifyForgotPasswordRequest)))
            {
                // Also manually check combined OTP input if necessary
                if (string.IsNullOrEmpty(VerifyForgotPasswordRequest.OTP) || VerifyForgotPasswordRequest.OTP.Length != 6 || !VerifyForgotPasswordRequest.OTP.All(char.IsDigit))
                {
                    // Add model error specifically for OTP if it wasn't caught by TryValidateModel
                    if (!ModelState.ContainsKey(nameof(VerifyForgotPasswordRequest) + ".OTP"))
                    {
                        ModelState.AddModelError(nameof(VerifyForgotPasswordRequest) + ".OTP", "OTP ph?i có 6 ch? s?.");
                    }
                }
                return new JsonResult(new { success = false, message = GetModelErrors(nameof(VerifyForgotPasswordRequest)) });
            }

            try
            {
                // Pass the DTO containing OTP and NewPassword
                var rs = await _authService.VerifyForgotPassword(VerifyForgotPasswordRequest);
                if (rs)
                {
                    GeneralSuccessMessage = "??t l?i m?t kh?u thành công! Vui lòng ??ng nh?p.";
                    return new JsonResult(new { success = true, message = "??t l?i m?t kh?u thành công! Vui lòng ??ng nh?p." });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Mã OTP không h?p l?, ?ã h?t h?n ho?c có l?i x?y ra." });
                }
            }
            catch (Exception ex)
            {
                // TODO: Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i khi ??t l?i m?t kh?u." });
            }
        }
    }

}



