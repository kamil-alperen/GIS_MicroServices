using GIS.Common;
using GIS.Contracts.City;
using GIS.Country.Service.Entities;
using MassTransit;

namespace GIS.Country.Service.Consume
{
    public class CityUpdatedConsumer : IConsumer<CityUpdated>
    {
        private readonly IRepository<CountryEntity> repository;

        public CityUpdatedConsumer(IRepository<CountryEntity> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CityUpdated> context)
        {
            CityUpdated city = context.Message;

            if (city.previousCountryId != city.countryId)
            {
                Result<CountryEntity> result = await repository.GetAsync(country => country.Id == city.previousCountryId);
                CountryEntity? previousCountry = result.IsSuccess ? result.Value : null;

                if (previousCountry != null)
                {
                    CityEntity? removedCity = previousCountry.Cities.Find(c => c.Id == city.id);
                    previousCountry.Cities.Remove(removedCity);

                    result = await repository.GetAsync(country => country.Id == city.countryId);
                    CountryEntity? country = result.IsSuccess ? result.Value : null;

                    if (country != null)
                    {
                        await repository.PutAsync(previousCountry);

                        if (country.Cities == null)
                            country.Cities = new List<CityEntity>();

                        country.Cities.Add(new CityEntity()
                        {
                            Id = city.id,
                            Name = city.cityName,
                            Population = city.cityPopulation
                        });

                        await repository.PutAsync(country);
                    }
                }
            }
            else
            {
                Result<CountryEntity> result = await repository.GetAsync(country => country.Id == city.countryId);
                CountryEntity? country = result.IsSuccess ? result.Value : null;

                if (country != null)
                {
                    country.Cities.Find(c => c.Id == city.id).Name = city.cityName;
                    country.Cities.Find(c => c.Id == city.id).Population = city.cityPopulation;

                    await repository.PutAsync(country);
                }
            }

        }
    }
}
