using RentalCar_System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.BaseService
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IBaseRepository<T> _baseRepository;

        public BaseService(IBaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<T> GetById(Guid id)
        {
            return await _baseRepository.GetByIdAsync(id);
        }

        public async Task<T> GetByUserName(string username)
        {
            return await _baseRepository.GetByUserNameAsync(username);
        }

        public async Task<T> SearchByName(string name)
        {
            return await _baseRepository.SearchByNameAsync(name);
        }
    }
}
