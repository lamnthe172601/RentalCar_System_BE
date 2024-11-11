using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Data
{
    public interface IBaseRepository<T> where T : class
    {
       
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<T> SearchByNameAsync(string name);
        Task<T> GetByEmailAsync(string email);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}
