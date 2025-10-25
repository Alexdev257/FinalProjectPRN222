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

        [BindProperty]
        public LoginRequest LoginRequest { get; set; } = new(); // Initialize for safety

        [BindProperty]
        public RegisterRequest RegisterRequest { get; set; } = new();

        [BindProperty]
        public VerifyRegisterRequest VerifyRegisterRequest { get; set; } = new();

        [BindProperty]
        public ForgotPasswordRequest ForgotPasswordRequest { get; set; } = new();

        [BindProperty]
        public VerifyForgotPasswordRequest VerifyForgotPasswordRequest { get; set; } = new();

        // --- TempData for SUCCESS messages after redirects ---
        [TempData]
        public string LoginSuccessMessage { get; set; } // Only used if login leads to redirect
        [TempData]
        public string GeneralSuccessMessage { get; set; } // Used after OTP success -> redirect to login

        // --- TempData for ERROR messages (fallback if JS fails or page reloads) ---
        [TempData]
        public string RegisterErrorMessage { get; set; }
        [TempData]
        public string LoginErrorMessage { get; set; }
        [TempData]
        public string ForgotPasswordErrorMessage { get; set; }
        [TempData]
        public string VerifyRegisterErrorMessage { get; set; }
        [TempData]
        public string VerifyForgotPasswordErrorMessage { get; set; }

        public void OnGet()
        {
        }

        // --- AJAX Handlers ---

        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, message = string.Join(" ", errors) });
            }
            try
            {
                var rs = await _authService.Login(LoginRequest);
                if (rs)
                {
                    // TODO: Implement actual sign-in logic (e.g., HttpContext.SignInAsync)
                    LoginSuccessMessage = "??ng nh?p thành công!"; // Set TempData for next page
                    return new JsonResult(new { success = true, redirectUrl = Url.Page("/Home") });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Email ho?c m?t kh?u không chính xác." });
                }
            }
            catch (Exception ex) // Catch specific exceptions if possible
            {
                // Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i h? th?ng. Vui lòng th? l?i sau." });
            }
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, message = string.Join(" ", errors) });
            }
            try
            {
                var rs = await _authService.Register(RegisterRequest);
                if (rs)
                {
                    // Return success and the email used, so JS can display it
                    return new JsonResult(new { success = true, email = RegisterRequest.Email });
                }
                else
                {
                    // Assuming service returns false if email exists or email failed
                    return new JsonResult(new { success = false, message = "Email này ?ã t?n t?i ho?c không th? g?i email xác th?c." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i h? th?ng khi ??ng ký. Vui lòng th? l?i." });
            }
        }

        // Renamed handler to match the form submission logic
        public async Task<IActionResult> OnPostVerifyRegisterAsync()
        {
            // Make sure the OTP from the hidden field is correctly bound
            if (string.IsNullOrEmpty(VerifyRegisterRequest.OTP) || VerifyRegisterRequest.OTP.Length != 6 || !VerifyRegisterRequest.OTP.All(char.IsDigit))
            {
                ModelState.AddModelError("VerifyRegisterRequest.OTP", "OTP ph?i có 6 ch? s?.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, message = string.Join(" ", errors) });
            }
            try
            {
                // Ensure AuthService.VerifyRegister uses the DTO correctly
                var rs = await _authService.VerifyRegsiter(VerifyRegisterRequest);
                if (rs)
                {
                    GeneralSuccessMessage = "Xác th?c tài kho?n thành công! Vui lòng ??ng nh?p."; // Set TempData for next page (login)
                    return new JsonResult(new { success = true, message = "Xác th?c thành công! Vui lòng ??ng nh?p." });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Mã OTP không h?p l? ho?c ?ã h?t h?n." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i khi xác th?c OTP. Vui lòng th? l?i." });
            }
        }

        public async Task<IActionResult> OnPostForgotPasswordAsync()
        {
            // Only validate the email field for this specific request DTO
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.Where(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
                // Check specifically if the error is for the Email field in this DTO
                if (ModelState.ContainsKey("ForgotPasswordRequest.Email") && ModelState["ForgotPasswordRequest.Email"].Errors.Any())
                {
                    return new JsonResult(new { success = false, message = string.Join(" ", errors) });
                }
                // If other model errors exist (shouldn't happen here), treat as success to avoid confusion, or handle differently.
                // For now, let's assume only email validation matters here. Proceed if email is valid.
            }

            try
            {
                // Ensure AuthService.ForgotPassword uses the correct DTO (only Email)
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
                // Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i h? th?ng. Vui lòng th? l?i." });
            }
        }

        public async Task<IActionResult> OnPostVerifyForgotPasswordAsync()
        {
            // Make sure the OTP from the hidden field is correctly bound
            if (string.IsNullOrEmpty(VerifyForgotPasswordRequest.OTP) || VerifyForgotPasswordRequest.OTP.Length != 6 || !VerifyForgotPasswordRequest.OTP.All(char.IsDigit))
            {
                ModelState.AddModelError("VerifyForgotPasswordRequest.OTP", "OTP ph?i có 6 ch? s?.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, message = string.Join(" ", errors) });
            }
            try
            {
                // Ensure AuthService.VerifyForgotPassword uses the DTO correctly (OTP + NewPassword)
                var rs = await _authService.VerifyForgotPassword(VerifyForgotPasswordRequest);
                if (rs)
                {
                    GeneralSuccessMessage = "??t l?i m?t kh?u thành công! Vui lòng ??ng nh?p."; // Set TempData for next page (login)
                    return new JsonResult(new { success = true, message = "??t l?i m?t kh?u thành công! Vui lòng ??ng nh?p." });
                }
                else
                {
                    // Service might return false for invalid OTP or DB update failure
                    return new JsonResult(new { success = false, message = "Mã OTP không h?p l?, ?ã h?t h?n ho?c có l?i x?y ra." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return new JsonResult(new { success = false, message = "?ã x?y ra l?i khi ??t l?i m?t kh?u. Vui lòng th? l?i." });
            }
        }
    }

}



