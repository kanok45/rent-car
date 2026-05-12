namespace CarRentingSystem.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Rental
    {
        public int Id { get; init; }

        public int CarId { get; set; }

        public Car Car { get; init; }

        public string RenterId { get; set; }

        public User Renter { get; init; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public decimal TotalPrice { get; set; }
        public bool IsActive { get; set; } = true;

        // Whether the dealer approved this rental
        public bool IsApproved { get; set; } = false;

        public DateTime? ApprovedOn { get; set; }
    }
}
