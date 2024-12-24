using GIS.Common;
using GIS.CityContracts;
using GIS.Country.Service.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace GIS.Country.Service.Consume
{
    public class CityDeletedConsumer : IConsumer<CityDeleted>
    {
        private readonly IRepository<CountryEntity> repository;
        private readonly ILogger<CityDeletedConsumer> _logger;

        public CityDeletedConsumer(IRepository<CountryEntity> repository, ILogger<CityDeletedConsumer> logger)
        {
            this.repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CityDeleted> context)
        {
            _logger.LogInformation("CityDeleted is consumed");

            CityDeleted city = context.Message;

            Result<CountryEntity> result = await repository.GetAsync(c => c.Id == city.countryId);

            if (result.IsSuccess)
            {
                CountryEntity? country = result.Value;

                if (country.Cities == null || country.Cities.Find(c => c.Id == city.id) == null)
                {
                    _logger.LogError(new ObjectResult(Results.Problem(
                        title: "CityDeleted city could not found",
                        extensions: new Dictionary<string, object?>
                        {
                            { "errors", new[] { result.Error } }
                        }
                    )).ToString());
                }
                else
                {
                    country.Cities.Remove(country.Cities.Find(c => c.Id == city.id));

                    Result<bool> updated = await repository.PutAsync(country);

                    if (updated.IsFailure)
                    {
                        _logger.LogError(new ObjectResult(Results.Problem(
                            title: "CityDeleted country could not updated",
                            extensions: new Dictionary<string, object?>
                            {
                                { "errors", new[] { result.Error } }
                            }
                        )).ToString());
                    }
                }
            }
            else
            {
                _logger.LogError(new ObjectResult(Results.Problem(
                        title: "CityDeleted country could not found",
                        extensions: new Dictionary<string, object?>
                        {
                            { "errors", new[] { result.Error } }
                        }
                    )).ToString());
            }
        }
    }
}
