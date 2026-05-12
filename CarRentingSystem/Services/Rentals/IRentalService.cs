namespace CarRentingSystem.Services.Rentals
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CarRentingSystem.Services.Rentals.Models;

    public interface IRentalService
    {
        Task<bool> IsCarAvailableAsync(int carId, DateTime startDate, DateTime endDate);

        Task<int> CreateRentalAsync(int carId, string userId, DateTime startDate, int days);

        Task CompleteRentalAsync(int rentalId);
        
        Task<IEnumerable<RentalServiceModel>> GetRentalsByUserAsync(string userId);

        Task<IEnumerable<DealerRentalServiceModel>> GetPendingRentalsForDealerAsync(int dealerId);

        Task ApproveRentalAsync(int rentalId, int dealerId);
    }
}
