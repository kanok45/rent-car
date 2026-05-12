namespace CarRentingSystem.Services.Rentals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CarRentingSystem.Data;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Services.Rentals.Models;
    using Microsoft.EntityFrameworkCore;

    public class RentalService : IRentalService
    {
        private readonly CarRentingDbContext db;

        public RentalService(CarRentingDbContext db)
        {
            this.db = db;
        }

        public async Task<bool> IsCarAvailableAsync(int carId, DateTime startDate, DateTime endDate)
        {
            return !await this.db.Rentals
                .AsNoTracking()
                .Where(r => r.CarId == carId && r.IsActive)
                .AnyAsync(r => r.StartDate < endDate && startDate < r.EndDate);
        }

        public async Task<int> CreateRentalAsync(int carId, string userId, DateTime startDate, int days)
        {
            var car = await this.db.Cars.FindAsync(carId);
            if (car == null)
            {
                throw new ArgumentException("Invalid car.");
            }

            var endDate = startDate.AddDays(days);

            var available = await this.IsCarAvailableAsync(carId, startDate, endDate);
            if (!available)
            {
                throw new InvalidOperationException("Car is not available for the selected dates.");
            }

            var rental = new Rental
            {
                CarId = carId,
                RenterId = userId,
                StartDate = startDate,
                EndDate = endDate,
                TotalPrice = car.PricePerDay * days,
                IsActive = true,
                IsApproved = false,
            };

            this.db.Rentals.Add(rental);
            await this.db.SaveChangesAsync();

            return rental.Id;
        }

        public async Task CompleteRentalAsync(int rentalId)
        {
            var rental = await this.db.Rentals.FindAsync(rentalId);
            if (rental == null)
            {
                throw new ArgumentException("Invalid rental id.");
            }

            rental.IsActive = false;
            this.db.Rentals.Update(rental);
            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<RentalServiceModel>> GetRentalsByUserAsync(string userId)
        {
            return await this.db.Rentals
                .Where(r => r.RenterId == userId)
                .OrderByDescending(r => r.StartDate)
                .Select(r => new RentalServiceModel
                {
                    Id = r.Id,
                    CarId = r.CarId,
                    CarTitle = r.Car.Brand + " " + r.Car.Model,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    TotalPrice = r.TotalPrice,
                    IsApproved = r.IsApproved
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DealerRentalServiceModel>> GetPendingRentalsForDealerAsync(int dealerId)
        {
            return await this.db.Rentals
                .Where(r => !r.IsApproved && r.IsActive && r.Car.DealerId == dealerId)
                .OrderBy(r => r.StartDate)
                .Select(r => new DealerRentalServiceModel
                {
                    Id = r.Id,
                    CarId = r.CarId,
                    CarTitle = r.Car.Brand + " " + r.Car.Model,
                    RenterId = r.RenterId,
                    RenterName = r.Renter.FullName,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    TotalPrice = r.TotalPrice
                })
                .ToListAsync();
        }

        public async Task ApproveRentalAsync(int rentalId, int dealerId)
        {
            var rental = await this.db.Rentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == rentalId);

            if (rental == null || rental.Car.DealerId != dealerId)
            {
                throw new ArgumentException("Invalid rental or permission denied.");
            }

            rental.IsApproved = true;
            rental.ApprovedOn = DateTime.UtcNow;
            this.db.Rentals.Update(rental);
            await this.db.SaveChangesAsync();
        }
    }
}
