using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Name is not empty!")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is not empty!")]
        [EmailAddress(ErrorMessage = "Email invalid!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is not empty!")]
        [MinLength(8, ErrorMessage = "Password contains at least 8 characters!")]
        public string Password　{ get; set; } = null!;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống!")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
