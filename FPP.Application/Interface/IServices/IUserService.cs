using FPP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();
        Task<User?> GetById(int id);
        Task<bool> Add(User user);
        Task<bool> Update(User user);
        Task<bool> Delete(int id);
    }
}
