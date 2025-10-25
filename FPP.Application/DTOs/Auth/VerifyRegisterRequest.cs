using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.DTOs.Auth
{
    public class VerifyRegisterRequest
    {
        [Required(ErrorMessage = "OTP is not empty!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digit!")]
        public string OTP { get; set; } = null!;
    }
}
