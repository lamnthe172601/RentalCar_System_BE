using RentalCar_System.Models.DtoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.QueueService
{
    public interface IQueueService
    {
        void Enqueue(AddToCartRequest request);
        AddToCartRequest Dequeue();
        bool IsQueueEmpty();
    }
}
