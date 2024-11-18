using RentalCar_System.Models.DtoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.QueueService
{
    public class QueueService : IQueueService
    {
        private readonly Queue<AddToCartRequest> _queue = new Queue<AddToCartRequest>();

        public void Enqueue(AddToCartRequest request)
        {
            _queue.Enqueue(request);
        }

        public AddToCartRequest Dequeue()
        {
            return _queue.Dequeue();
        }

        public bool IsQueueEmpty()
        {
            return _queue.Count == 0;
        }
    }
}
