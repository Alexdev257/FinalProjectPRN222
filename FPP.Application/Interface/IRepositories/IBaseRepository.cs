using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        IQueryable<T> GetAllAsync();
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
