﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.BaseService
{
    public interface IBaseService<T> where T : class
    {

        Task<T> GetByIdAsync(Guid id);       
        Task<T> SearchByNameAsync(string name);
    }
}
