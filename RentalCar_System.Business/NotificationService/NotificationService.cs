using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RentalCar_System.Business.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly string _smtpHost = "smtp.gmail.com"; 
        private readonly int _smtpPort = 587; 
        private readonly string _smtpUsername = "fanjfla1989@gmail.com"; 
        private readonly string _smtpPassword = "gtfr fkwx kcvx olyr"; 

        public async Task SendEmailNotificationAsync(string email, string subject, string message)
        {
            try
            {
                var smtpClient = new SmtpClient(_smtpHost)
                {
                    Port = _smtpPort,
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUsername),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
            }
        }
    }
}
