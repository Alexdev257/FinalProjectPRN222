using FPP.Application.DTOs.Auth;
using FPP.Application.Interface.IHelpers;
using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Infrastructure.Implements.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisHelper _redisHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IOtpHelper _otpHelper;
        private readonly IBcryptHelper _bcryptHelper;

        public AuthService(IUnitOfWork unitOfWork, IRedisHelper redisHelper, IEmailHelper emailHelper, IOtpHelper otpHelper, IBcryptHelper bcryptHelper)
        {
            _unitOfWork = unitOfWork;
            _redisHelper = redisHelper;
            _emailHelper = emailHelper;
            _otpHelper = otpHelper;
            _bcryptHelper = bcryptHelper;
        }

        public async Task<User?> Login(LoginRequest request)
        {
            var userList = await _unitOfWork.Users.GetAllAsync()
                .Where(u => u.Email == request.Email).ToListAsync();

            var user = userList.FirstOrDefault();
            if(user == null) return null;
            if (!_bcryptHelper.VerifyPassword(request.Password, user.PasswordHash)) return null;
            return user;
            
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            bool rs;
            var userList = await _unitOfWork.Users.GetAllAsync()
                .Where(u => u.Email == request.Email).ToListAsync();

            var user = userList.FirstOrDefault();
            if (user != null)
            {
                rs = false;
            }
            else
            {
                var otp = _otpHelper.GenerateOtpAsync(6);
                var dictionary = new Dictionary<string, string>
                {
                    { "Email", request.Email },
                    { "otp", otp }
                };
                bool result = await _emailHelper.SendEmailAsync(request.Email, "Register", $"Hello Email. Your OTP code is: otp", dictionary);
                if (!result)
                {
                    rs = false;
                }
                else
                {
                    await _redisHelper.SetAsync($"RO_{otp}", request, TimeSpan.FromMinutes(5));
                    //await _redisHelper.SetAsync($"ROtp_{otp}", otp, TimeSpan.FromMinutes(5));
                    rs = true;
                }    
            }
            return rs;
        }

        public async Task<bool> VerifyRegsiter(VerifyRegisterRequest request)
        {
            var user = await _redisHelper.GetAsync<RegisterRequest>($"RO_{request.OTP}");
            if (user == null) return false;

            var userEntity = new User
            {
                Name = user.Name,
                Email = user.Email,
                PasswordHash = _bcryptHelper.HashPassword(user.Password),
                Role = 1,
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.Users.Add(userEntity);
            var cltrs = await _unitOfWork.CompleteAsync();
            return cltrs;
        }

        public async Task<bool> ForgotPassword(ForgotPasswordRequest request)
        {
            var userList = await _unitOfWork.Users.GetAllAsync()
                .Where(u => u.Email == request.Email).ToListAsync();

            var user = userList.FirstOrDefault();
            if (user == null) return false;
            var otp = _otpHelper.GenerateOtpAsync(6);
            var dictionary = new Dictionary<string, string>
            {
                { "Email", request.Email },
                { "otp", otp }
            };
            bool result = await _emailHelper.SendEmailAsync(request.Email, "ForgotPassword", $"Hello Email. Your OTP code is: otp", dictionary);
            if (!result)
            {
                return false;
            }
            else
            {
                await _redisHelper.SetAsync($"FPO_{otp}", request, TimeSpan.FromMinutes(5));
                //await _redisHelper.SetAsync($"FPOtp_{otp}", otp, TimeSpan.FromMinutes(5));
                return true;
            }
        }

        public async Task<bool> VerifyForgotPassword(VerifyForgotPasswordRequest request)
        {
            var data = await _redisHelper.GetAsync<ForgotPasswordRequest>($"FPO_{request.OTP}");
            var user = await _unitOfWork.Users.GetAllAsync()
                .Where(u => u.Email == data!.Email).FirstOrDefaultAsync();

            user.PasswordHash = _bcryptHelper.HashPassword(data.NewPassword);
            _unitOfWork.Users.Update(user);
            return await _unitOfWork.CompleteAsync();
        }

        
    }
}
