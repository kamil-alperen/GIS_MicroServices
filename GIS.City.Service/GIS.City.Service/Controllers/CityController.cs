using System.Diagnostics.Metrics;
using GIS.City.Service.DTOs;
using GIS.City.Service.Entities;
using GIS.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using GIS.CityContracts;
using MassTransit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using GIS.City.Service.Client;
using GIS.City.Service.Business;

namespace GIS.City.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CityController : Controller
    {
        private readonly ILogger<CityController> _logger;
        private readonly IRepository<CityEntity> _repository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly CountryClient _countryClient;
        public CityController(ILogger<CityController> logger, IRepository<CityEntity> repository, IPublishEndpoint publishEndpoint, CountryClient countryClient) {
            _logger = logger;
            _repository = repository;
            _publishEndpoint = publishEndpoint;
            _countryClient = countryClient;
        }

        [HttpGet("GetIds", Name = "GetIds")]
        public async Task<IActionResult> GetIdsAsync()
        {
            Result<IReadOnlyCollection<CityEntity>> result = await _repository.GetAllAsync();

            return result.IsSuccess ? Ok(result.Value?.Select(ent => ent.AsCity()).ToList()) : new ObjectResult(Results.Problem(
                    title: "Error occurred",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        { "errors", new[] { result.Error } }
                    }
                ));
        }

        [HttpGet("GetAll", Name = "GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            Result<IReadOnlyCollection<CityEntity>> result = await _repository.GetAllAsync();

            return result.IsSuccess ? Ok(result.Value?.Select(ent => ent.AsCityDTO()).ToList()) : new ObjectResult(Results.Problem(
                    title: "Error occurred",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        { "errors", new[] { result.Error } }
                    }
                ));
        }

        [HttpGet("Get", Name = "Get")]
        public async Task<IActionResult> Get(Guid id)
        {
            Result<CityEntity> result = await _repository.GetAsync(id);

            return result.IsSuccess ? Ok(result.Value?.AsCityDTO()) : new ObjectResult(Results.Problem(
                    title: "Error occurred",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        { "errors", new[] { result.Error } }
                    }
                ));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityCreateDTO cityCreateDTO)
        {
            CityEntity cityEntity = new CityEntity()
            {
                Id = cityCreateDTO.id,
                CityName = cityCreateDTO.cityName,
                CityPopulation = cityCreateDTO.cityPopulation,
                CountryId = cityCreateDTO.countryId
            };

            bool countryExists = await CityBusiness.CountryExists(_countryClient, cityCreateDTO.countryId);

            Result<CityEntity> result = await _repository.PostAsync(cityEntity);

            if (result.IsSuccess && countryExists)
            {
                CityEntity? createdEntity = result.Value;
                await _publishEndpoint.Publish(new CityCreated(createdEntity.Id, createdEntity.CityName, createdEntity.CountryId, createdEntity.CityPopulation));
                return Ok(createdEntity.AsCityDTO());
            }
            else
            {
                return new ObjectResult(Results.Problem(
                        title: "Error occurred",
                        statusCode: StatusCodes.Status400BadRequest,
                        extensions: new Dictionary<string, object?>
                        {
                            { "errors", new[] { result.Error } }
                        }
                    ));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCity([FromBody] CityUpdateDTO cityUpdateDTO)
        {
            CityEntity cityEntity = new CityEntity()
            {
                Id = cityUpdateDTO.id,
                CityName = cityUpdateDTO.cityName,
                CityPopulation = cityUpdateDTO.cityPopulation,
                CountryId = cityUpdateDTO.countryId
            };

            bool countryExists = await CityBusiness.CountryExists(_countryClient, cityUpdateDTO.countryId);

            Result<CityEntity> previousCity = await _repository.GetAsync(cityEntity.Id);

            if (previousCity.IsSuccess && countryExists)
            {
                Result<bool> updateResult = await _repository.PutAsync(cityEntity);

                if (updateResult.IsSuccess)
                {
                    await _publishEndpoint.Publish(new CityUpdated(cityEntity.Id, cityEntity.CityName, previousCity.Value.CountryId, cityEntity.CountryId, cityEntity.CityPopulation, null));
                    return Ok(updateResult.Value);
                }
                else
                {
                    return new ObjectResult(Results.Problem(
                        title: "Error occurred",
                        statusCode: StatusCodes.Status400BadRequest,
                        extensions: new Dictionary<string, object?>
                        {
                                { "errorsUpdateCity", new[] { updateResult.Error } }
                        }
                    ));
                }
            }
            else
            {
                return new ObjectResult(Results.Problem(
                        title: "Error occurred",
                        statusCode: StatusCodes.Status400BadRequest,
                        extensions: new Dictionary<string, object?>
                        {
                                { "errorsPreviousCity", new[] { previousCity.Error } }
                        }
                    ));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCity([FromQuery] string cityName)
        {
            Result<CityEntity> result = await _repository.GetAsync(entity => entity.CityName == cityName);

            if (result.IsSuccess)
            {
                CityEntity? cityEntity = result.Value;
                Result<bool> deleteResult = await _repository.DeleteAsync(cityEntity.Id);
                bool deleted = deleteResult.IsSuccess ? deleteResult.Value : false;

                if (deleted)
                {
                    await _publishEndpoint.Publish(new CityDeleted(cityEntity.Id, cityEntity.CountryId));
                    return Ok();
                }
                else
                {
                    return new ObjectResult(Results.Problem(
                            title: "Error occurred",
                            statusCode: StatusCodes.Status400BadRequest,
                            extensions: new Dictionary<string, object?>
                            {
                                { "errors", new[] { deleteResult.Error } }
                            }
                        ));
                }
            }
            else
            {
                return new ObjectResult(Results.Problem(
                            title: "Error occurred",
                            statusCode: StatusCodes.Status400BadRequest,
                            extensions: new Dictionary<string, object?>
                            {
                                { "errors", new[] { result.Error } }
                            }
                        ));
            }
        }


    }
}
