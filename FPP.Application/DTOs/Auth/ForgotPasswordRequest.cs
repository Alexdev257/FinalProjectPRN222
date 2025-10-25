using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is not empty!")]
        [EmailAddress(ErrorMessage = "Email invalid!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "New password is not empty!")]
        [MinLength(8, ErrorMessage = "Password contains at least 8 characters!")]
        public string NewPassword { get; set; } = null!;
    }
}
