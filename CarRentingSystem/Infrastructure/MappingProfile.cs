namespace CarRentingSystem.Infrastructure
{
    using AutoMapper;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Models.Cars;
    using CarRentingSystem.Services.Cars.Models;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Category, CarCategoryServiceModel>();

            this.CreateMap<CarDetailsServiceModel, CarFormModel>();

            this.CreateMap<Car, LatestCarServiceModel>()
                .ForMember(c => c.PricePerDay, cfg => cfg.MapFrom(c => c.PricePerDay));

            this.CreateMap<Car, CarServiceModel>()
                .ForMember(c => c.CategoryName, cfg => cfg.MapFrom(c => c.Category.Name))
                .ForMember(c => c.PricePerDay, cfg => cfg.MapFrom(c => c.PricePerDay));

            this.CreateMap<Car, CarDetailsServiceModel>()
                .ForMember(c => c.UserId, cfg => cfg.MapFrom(c => c.Dealer.UserId))
                .ForMember(c => c.CategoryName, cfg => cfg.MapFrom(c => c.Category.Name))
                .ForMember(c => c.PricePerDay, cfg => cfg.MapFrom(c => c.PricePerDay));
        }
    }
}
