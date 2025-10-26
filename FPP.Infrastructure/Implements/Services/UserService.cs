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
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _unitOfWork.Users.GetAllAsync().ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }
        public async Task<bool> Add(User user)
        {
            _unitOfWork.Users.Add(user);
            return await _unitOfWork.CompleteAsync();
        }
        public async Task<bool> Update(User user)
        {
            _unitOfWork.Users.Update(user);
            return await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            _unitOfWork.Users.Remove(user!);
            return await _unitOfWork.CompleteAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(decimal role)
        {
            return await _unitOfWork.Users.GetAllAsync()
                .Where(u => u.Role == role)
                .ToListAsync();
        }


    }
}
