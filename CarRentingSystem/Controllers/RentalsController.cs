namespace CarRentingSystem.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using CarRentingSystem.Services.Rentals;
    using CarRentingSystem.Services.Rentals.Models;
    using CarRentingSystem.Services.Dealers;
    using CarRentingSystem.Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class RentalsController : Controller
    {
        private readonly IRentalService rentals;
        private readonly IDealerService dealers;

        public RentalsController(IRentalService rentals, IDealerService dealers)
        {
            this.rentals = rentals;
            this.dealers = dealers;
        }

        // User dashboard: show user's rentals
        public async Task<IActionResult> MyRentals()
        {
            var userId = this.User.Id();
            var model = await this.rentals.GetRentalsByUserAsync(userId);
            return this.View(model);
        }

        // Dealer dashboard: pending rentals for dealer's cars
        public async Task<IActionResult> Pending()
        {
            var userId = this.User.Id();
            var dealerId = this.dealers.IdByUser(userId);
            if (dealerId == 0)
            {
                return RedirectToAction("Become", "Dealers");
            }

            var model = await this.rentals.GetPendingRentalsForDealerAsync(dealerId);
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var userId = this.User.Id();
            var dealerId = this.dealers.IdByUser(userId);
            if (dealerId == 0)
            {
                return RedirectToAction("Become", "Dealers");
            }

            try
            {
                await this.rentals.ApproveRentalAsync(id, dealerId);
            }
            catch
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Pending));
        }
    }
}
