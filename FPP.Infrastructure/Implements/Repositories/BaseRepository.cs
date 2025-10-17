using FPP.Application.Interface.IRepositories;
using FPP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Infrastructure.Implements.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public IQueryable<T> GetAllAsync()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _context.ChangeTracker.Clear();
            _dbSet.Update(entity);
            //return await _context.SaveChangesAsync() > 0;
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
            //return await _context.SaveChangesAsync() > 0;
        }
    }
}
