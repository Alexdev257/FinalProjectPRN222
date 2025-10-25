using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is not empty!")]
        [EmailAddress(ErrorMessage = "Email invalid!")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is not empty!")]
        [MinLength(8, ErrorMessage = "Password contains at least 8 characters!")]
        public string Password { get; set; } = null!;
    }
}
