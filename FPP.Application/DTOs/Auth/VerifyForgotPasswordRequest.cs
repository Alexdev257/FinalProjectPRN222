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
        public string OTP { get; set; } = null!;
    }
}
