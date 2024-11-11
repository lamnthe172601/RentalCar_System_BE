using RentalCar_System.Business.BaseService;
using RentalCar_System.Data;
using RentalCar_System.Data.CarRepository;
using RentalCar_System.Data.RentalContractRepository;
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
        public RentalContractService(IRentalContractRepository rentalContractRepository , ICarRepository carRepository)
        {
            _rentalContractRepository = rentalContractRepository;
            _carRepository = carRepository;
        }

        public async Task<IEnumerable<RentalContract>> GetAllContractsByUserIdAsync(Guid userId)
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
    }
}
