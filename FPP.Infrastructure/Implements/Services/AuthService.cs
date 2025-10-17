using FPP.Application.DTOs;
using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
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

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Login(LoginRequest request)
        {
            var rs = false;
            var userList = await _unitOfWork.Users.GetAllAsync()
                .Where(u => u.Email == request.Email).ToListAsync();

            var user = userList.FirstOrDefault();
            if(user == null) return false;
            if (user.PasswordHash != request.Password) return false;
            rs = true;
            Console.WriteLine(rs);
            return rs;
            
        }

        public Task<bool> Register(LoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
