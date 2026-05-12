namespace CarRentingSystem.Controllers
{
    using System;
    using System.Threading.Tasks;
    using CarRentingSystem.Data;
    using CarRentingSystem.Infrastructure.Extensions;
    using CarRentingSystem.Services.Rentals;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Authorize]
    public class RentController : Controller
    {
        private readonly CarRentingDbContext db;
        private readonly IRentalService rentals;

        public RentController(CarRentingDbContext db, IRentalService rentals)
        {
            this.db = db;
            this.rentals = rentals;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int carId)
        {
            var car = await this.db.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == carId);

            if (car == null)
            {
                return NotFound();
            }

            var model = new RentInputModel
            {
                CarId = car.Id,
                CarDescription = $"{car.Brand} {car.Model}",
                PricePerDay = car.PricePerDay,
                StartDate = DateTime.Today,
                Days = 1,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RentInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var userId = this.User.Id();
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Create rental
            try
            {
                await this.rentals.CreateRentalAsync(model.CarId, userId, model.StartDate, model.Days);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return this.View(model);
            }

            return this.RedirectToAction("MyRentals", "Rentals");
        }

        public class RentInputModel
        {
            public int CarId { get; set; }

            public string CarDescription { get; set; }

            public decimal PricePerDay { get; set; }

            public DateTime StartDate { get; set; }

            public int Days { get; set; }
        }
    }
}
