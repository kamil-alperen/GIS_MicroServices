using GIS.Common;
using GIS.Contracts.City;
using GIS.Country.Service.Entities;
using MassTransit;

namespace GIS.Country.Service.Consume
{
    public class CityCreatedConsumer : IConsumer<CityCreated>
    {
        private readonly IRepository<CountryEntity> repository;

        public CityCreatedConsumer(IRepository<CountryEntity> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CityCreated> context)
        {
            CityCreated city = context.Message;

            CityEntity cityEntity = new CityEntity
            {
                Id = city.id,
                Name = city.cityName,
                Population = city.cityPopulation
            };

            Guid countryId = city.countryId;

            Result<CountryEntity> result = await repository.GetAsync(country => country.Id == countryId);
            CountryEntity? country = result.IsSuccess ? result.Value : null;

            if (country != null)
            {
                if (country.Cities == null)
                    country.Cities = new List<CityEntity>();

                country.Cities.Add(cityEntity);

                await repository.PutAsync(country);
            }
        }
    }
}
