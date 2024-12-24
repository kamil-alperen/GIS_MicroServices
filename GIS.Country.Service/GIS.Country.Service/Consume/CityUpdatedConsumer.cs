using GIS.Common;
using GIS.CityContracts;
using GIS.Country.Service.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using static MassTransit.ValidationResultExtensions;
using GIS.DistrictContracts;

namespace GIS.Country.Service.Consume
{
    public class CityUpdatedConsumer : IConsumer<CityUpdated>
    {
        private readonly IRepository<CountryEntity> repository;
        private readonly ILogger<CityUpdatedConsumer> _logger;

        public CityUpdatedConsumer(IRepository<CountryEntity> repository, ILogger<CityUpdatedConsumer> logger)
        {
            this.repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CityUpdated> context)
        {
            _logger.LogInformation("CityUpdated is consumed");

            CityUpdated cityUpdated = context.Message;

            if (cityUpdated.previousCountryId == cityUpdated.countryId)
            {
                Result<CountryEntity> countryResult = await repository.GetAsync(cityUpdated.countryId);

                if (countryResult.IsSuccess)
                {
                    CountryEntity? country = countryResult.Value;

                    if (country.Cities == null || country.Cities.Find(city => city.Id == cityUpdated.id) == null)
                    {
                        _logger.LogError(new ObjectResult(Results.Problem(
                            title: "CityDeleted city not found",
                            extensions: new Dictionary<string, object?>
                            {
                                { "errors", new[] { countryResult.Error } }
                            }
                        )).ToString());
                    }
                    else
                    {
                        CityEntity updateCity = country.Cities.Find(city => city.Id == cityUpdated.id);
                        updateCity = new CityEntity
                        {
                            Id = cityUpdated.id,
                            Name = cityUpdated.cityName != default ? cityUpdated.cityName : updateCity.Name,
                            Population = cityUpdated.cityPopulation != default ? cityUpdated.cityPopulation : updateCity.Population,
                        };

                        country.Cities[country.Cities.FindIndex(city => city.Id == cityUpdated.id)] = updateCity;

                        Result<bool> updated = await repository.PutAsync(country);

                        if (updated.IsFailure)
                        {
                            _logger.LogError(new ObjectResult(Results.Problem(
                                title: "CityDeleted country could not updated",
                                extensions: new Dictionary<string, object?>
                                {
                                    { "errors", new[] { countryResult.Error } }
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
                                { "errors", new[] { countryResult.Error } }
                            }
                        )).ToString());
                }
            }
            else
            {
                Result<CountryEntity> countryPrevious = await repository.GetAsync(cityUpdated.previousCountryId);
                Result<CountryEntity> countryCurrent = await repository.GetAsync(cityUpdated.countryId);

                if (countryPrevious.IsSuccess && countryCurrent.IsSuccess)
                {
                    CountryEntity? current = countryCurrent.Value;
                    CountryEntity? previous = countryPrevious.Value;

                    if (previous.Cities == null || previous.Cities.Find(c => c.Id == cityUpdated.id) == null)
                    {
                        _logger.LogError(new ObjectResult(Results.Problem(
                            title: "CityUpdated city could not found",
                            extensions: new Dictionary<string, object?>
                            {
                                { "errors", new[] { $"City:{cityUpdated.id} could not found" } }
                            }
                        )).ToString());
                    }
                    else
                    {
                        CityEntity cityEntity = previous.Cities.Find(d => d.Id == cityUpdated.id);
                        previous.Cities.Remove(cityEntity);
                        Result<bool> updatedPrevious = await repository.PutAsync(previous);

                        if (updatedPrevious.IsSuccess)
                        {
                            if (current.Cities == null)
                                current.Cities = new List<CityEntity>();

                            current.Cities.Add(cityEntity);

                            Result<bool> updatedCurrent = await repository.PutAsync(current);

                            if (updatedCurrent.IsFailure)
                            {
                                _logger.LogError(new ObjectResult(Results.Problem(
                                    title: "CityUpdated country could not updated",
                                    extensions: new Dictionary<string, object?>
                                    {
                                        { "errors", new[] { updatedCurrent.Error } }
                                    }
                                )).ToString());
                            }
                        }
                        else
                        {
                            _logger.LogError(new ObjectResult(Results.Problem(
                                title: "CityUpdated country could not updated",
                                extensions: new Dictionary<string, object?>
                                {
                                    { "errors", new[] { updatedPrevious.Error } }
                                }
                            )).ToString());
                        }
                    }
                }
                else
                {
                    _logger.LogError(new ObjectResult(Results.Problem(
                        title: "CityUpdated country could not found",
                        extensions: new Dictionary<string, object?>
                        {
                            { "errorsCurrent", new[] { countryCurrent.Error } },
                            { "errorsPrevious", new[] { countryPrevious.Error } }
                        }
                    )).ToString());
                }
            }
        }
    }
}
