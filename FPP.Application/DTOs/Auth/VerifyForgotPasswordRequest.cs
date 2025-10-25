using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.DTOs.Auth
{
    public class VerifyForgotPasswordRequest
    {
        [Required(ErrorMessage = "OTP is not empty!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digit!")]
        public string OTP { get; set; } = null!;
        [Required(ErrorMessage = "Mật khẩu mới không được để trống!")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự!")]
        public string NewPassword { get; set; } = null!;

        // THÊM: Confirm New Password
        [Required(ErrorMessage = "Xác nhận mật khẩu mới không được để trống!")]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
