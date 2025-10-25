using FPP.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IServices
{
    public interface IAuthService
    {
        Task<bool> Login(LoginRequest request);
        Task<bool> Register(RegisterRequest request);
        Task<bool> VerifyRegsiter(VerifyRegisterRequest request);
        Task<bool> ForgotPassword(ForgotPasswordRequest request);
        Task<bool> VerifyForgotPassword(VerifyForgotPasswordRequest request);
    }
}
