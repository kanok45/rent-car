namespace CarRentingSystem.Services.Rentals.Models
{
    using System;

    public class RentalServiceModel
    {
        public int Id { get; init; }

        public int CarId { get; init; }

        public string CarTitle { get; init; }

        public DateTime StartDate { get; init; }

        public DateTime EndDate { get; init; }

        public decimal TotalPrice { get; init; }

        public bool IsApproved { get; init; }
    }
}
