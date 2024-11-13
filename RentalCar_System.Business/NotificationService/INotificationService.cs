using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.NotificationService
{
    public interface INotificationService
    {
        Task SendEmailNotificationAsync(string email, string subject, string message);
    }
}
