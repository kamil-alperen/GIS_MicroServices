using GIS.Common;
using GIS.Contracts.City;
using GIS.Country.Service.Entities;
using MassTransit;

namespace GIS.Country.Service.Consume
{
    public class CityDeletedConsumer : IConsumer<CityDeleted>
    {
        private readonly IRepository<CountryEntity> repository;

        public CityDeletedConsumer(IRepository<CountryEntity> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CityDeleted> context)
        {
            CityDeleted city = context.Message;

            Result<CountryEntity> result = await repository.GetAsync(c => c.Id == city.countryId);
            CountryEntity? country = result.IsSuccess ? result.Value : null;

            if (country != null)
            {
                CityEntity? removedCity = country.Cities.Find(c => c.Id == city.id);
                country.Cities.Remove(removedCity);

                await repository.PutAsync(country);
            }
        }
    }
}
