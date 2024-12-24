using GIS.Common;
using GIS.CityContracts;
using GIS.Country.Service.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace GIS.Country.Service.Consume
{
    public class CityCreatedConsumer : IConsumer<CityCreated>
    {
        private readonly IRepository<CountryEntity> repository;
        private readonly ILogger<CityCreatedConsumer> _logger;  

        public CityCreatedConsumer(IRepository<CountryEntity> repository, ILogger<CityCreatedConsumer> logger)
        {
            this.repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CityCreated> context)
        {
            _logger.LogInformation($"CityCreated is consumed");

            CityCreated city = context.Message;

            CityEntity cityEntity = new CityEntity
            {
                Id = city.id,
                Name = city.cityName,
                Population = city.cityPopulation
            };

            Guid countryId = city.countryId;

            Result<CountryEntity> result = await repository.GetAsync(country => country.Id == countryId);

            if (result.IsSuccess)
            {
                CountryEntity? country = result.Value;

                if (country.Cities == null)
                    country.Cities = new List<CityEntity>();

                country.Cities.Add(cityEntity);

                Result<bool> created = await repository.PutAsync(country);

                if (created.IsFailure)
                {
                    _logger.LogError(new ObjectResult(Results.Problem(
                        title: "CityCreated country update error",
                        extensions: new Dictionary<string, object?>
                        {
                            { "errors", new[] { created.Error } }
                        }
                    )).ToString());
                }
            }
            else
            {
                _logger.LogError(new ObjectResult(Results.Problem(
                        title: "CityCreated country could not found",
                        extensions: new Dictionary<string, object?>
                        {
                            { "errors", new[] { result.Error } }
                        }
                    )).ToString());
            }
        }
    }
}
