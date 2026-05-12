namespace CarRentingSystem.Services.Rentals.Models
{
    using System;

    public class DealerRentalServiceModel
    {
        public int Id { get; init; }

        public int CarId { get; init; }

        public string CarTitle { get; init; }

        public string RenterId { get; init; }

        public string RenterName { get; init; }

        public DateTime StartDate { get; init; }

        public DateTime EndDate { get; init; }

        public decimal TotalPrice { get; init; }
    }
}
