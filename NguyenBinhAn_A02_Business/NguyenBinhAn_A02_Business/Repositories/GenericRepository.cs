using Microsoft.EntityFrameworkCore;
using NguyenBinhAn_A02_Data;
using System.Linq.Expressions;

namespace NguyenBinhAn_A02_Business.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly FUNewsManagementDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(FUNewsManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null;
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
