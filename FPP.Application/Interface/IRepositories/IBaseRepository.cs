using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        IQueryable<T> GetAllAsync(Expression<Func<T, bool>>? predicate = null);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);

        IQueryable<T> Query();
    }
}
