
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareAppointment.Business.BaseService
{
    public interface IBaseService<T> where T : class
    {

        Task<T> GetById(Guid id);
        Task<T> GetByUserName(string username);

        Task<T> SearchByName(string name);
    }
}
