using FPP.Application.DTOs;
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
        Task<bool> Register(LoginRequest request);
    }
}
