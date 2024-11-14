using RentalCar_System.Business.BaseService;
using RentalCar_System.Business.NotificationService;
using RentalCar_System.Data;
using RentalCar_System.Data.CarRepository;
using RentalCar_System.Data.RentalContractRepository;
using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.RentalCarService
{
    public class RentalContractService : IRentalContractService
    {
        private readonly IRentalContractRepository _rentalContractRepository;
        private readonly ICarRepository _carRepository;
        private readonly INotificationService _notificationService;
        public RentalContractService(IRentalContractRepository rentalContractRepository , ICarRepository carRepository , INotificationService notificationService)
        {
            _rentalContractRepository = rentalContractRepository;
            _carRepository = carRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<CarRented>> GetAllContractsByUserIdAsync(Guid userId)
        {
            return await _rentalContractRepository.GetAllContractsByUserIdAsync(userId);
        }

        public async Task<RentalContract> GetRentalContractByIdAsync(Guid contractId)
        {
            return await _rentalContractRepository.GetRentalContractByIdAsync(contractId);
        }

        public async Task<RentalContract> SendRentRequestAsync(Guid userId, Guid carId, DateTime rentalDate, DateTime returnDate)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null || car.Status != "Available")
            {
                throw new Exception("Car is not available.");
            }

            var rentalDays = (returnDate - rentalDate).Days;
            if (rentalDays <= 0)
            {
                throw new Exception("Invalid rental period.");
            }

            var totalAmount = rentalDays * car.Price;

            var rentalContract = new RentalContract
            {
                ContractId = Guid.NewGuid(),
                UserId = userId,
                CarId = carId,
                RentalDate = rentalDate,
                ReturnDate = returnDate,
                TotalAmount = totalAmount,
                Status = "Pending"  
            };

            await _rentalContractRepository.AddContractAsync(rentalContract);
            return rentalContract;
        }

        public async Task<bool> CancelRentalContractAsync(Guid contractId)
        {
            var contract = await _rentalContractRepository.GetRentalContractByIdAsync(contractId);

            if (contract == null)
            {
                throw new Exception("Rental contract not found.");
            }

            
            if (contract.Status != "Pending")
            {
                throw new Exception("Only pending contracts can be canceled.");
            }

            
            contract.Status = "Active";
            await _rentalContractRepository.UpdateContractAsync(contract);

            return true;
        }
        public async Task<bool> UpdateFeedbackAndRatingAsync(Guid contractId, string feedback, int rating)
        {
            var contract = await _rentalContractRepository.GetRentalContractByIdAsync(contractId);
            if (contract == null) return false;

            contract.Feedback = feedback;
            contract.Rating = rating;
            await _rentalContractRepository.UpdateContractAsync(contract);
            return true;
        }
        public async Task NotifyExpiringContractsAsync()
        {
            try
            {
               
                var expirationThreshold = TimeSpan.FromDays(2);
                var currentDate = DateTime.UtcNow;

               
                var contracts = await _rentalContractRepository.GetAllAsync();

                foreach (var contract in contracts)
                {
                    if (contract.ReturnDate.HasValue)
                    {
                        var daysRemaining = contract.ReturnDate.Value - currentDate;

                        
                        if (daysRemaining <= expirationThreshold && daysRemaining >= TimeSpan.Zero)
                        {
                            var user = await _rentalContractRepository.GetUserByContractIdAsync(contract.ContractId);

                          
                            if (user != null && !string.IsNullOrEmpty(user.Email))
                            {
                                var subject = "Reminder: Your rental car is due for return soon!";
                                var message = $"Dear {user.Name},\n\n" +
                                              $"Your rental car (Contract ID: {contract.ContractId}) is due for return on {contract.ReturnDate?.ToString("dd/MM/yyyy")}.\n" +
                                              "Please return the car on time to avoid any late fees.";

                                await _notificationService.SendEmailNotificationAsync(user.Email, subject, message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NotifyExpiringContracts: {ex.Message}");
            }
        }       
    }
}
