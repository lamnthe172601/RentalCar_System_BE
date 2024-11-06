
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RentalCar_System.Data;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RentalCar_System.Data
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {

        private readonly RentalCarDBContext _context;
        //private readonly DbSet<T> _DbSet;
        public BaseRepository(RentalCarDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
        {

            IQueryable<T> query = _context.Set<T>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            _context.Add<T>(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));


            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {

            var entity = await _context.Set<T>().FindAsync(id);

            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Entity with id {id} not found.");
            }
        }


        public async Task<T> SearchByNameAsync(string name)
        {

            return await _context.Set<T>().FindAsync(name);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByUserNameAsync(string username)
        {
            return await _context.Set<T>().FindAsync(username);
        }
    }
}