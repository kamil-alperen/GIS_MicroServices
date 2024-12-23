using System.Diagnostics.Metrics;
using GIS.City.Service.DTOs;
using GIS.City.Service.Entities;
using GIS.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using GIS.Contracts;
using GIS.Contracts.City;
using MassTransit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Driver;

namespace GIS.City.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityController : Controller
    {
        private readonly ILogger<CityController> _logger;
        private readonly IRepository<CityEntity> _repository;
        private readonly IPublishEndpoint _publishEndpoint;
        public CityController(ILogger<CityController> logger, IRepository<CityEntity> repository, IPublishEndpoint publishEndpoint) {
            _logger = logger;
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("GetAll", Name = "GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            Result<IReadOnlyCollection<CityEntity>> result = await _repository.GetAllAsync();

            return result.IsSuccess ? Ok(result.Value?.Select(ent => ent.AsCityDTO()).ToList()) : new ObjectResult(Results.Problem(
                    title: "Error occured",
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
                    title: "Error occured",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        { "errors", new[] { result.Error } }
                    }
                ));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity(CityDTO cityDTO)
        {
            CityEntity cityEntity = new CityEntity()
            {
                Id = cityDTO.id,
                CityName = cityDTO.cityName,
                CityPopulation = cityDTO.cityPopulation,
                CountryId = cityDTO.countryId
            };

            Result<CityEntity> result = await _repository.PostAsync(cityEntity);
            CityEntity? createdEntity = result.IsSuccess ? result.Value : null;

            if (createdEntity != null)
            {
                _publishEndpoint.Publish(new CityCreated(cityEntity.Id, cityEntity.CityName, cityEntity.CountryId, cityEntity.CityPopulation));
                return Ok(createdEntity.AsCityDTO());
            }
            else
            {
                return new ObjectResult(Results.Problem(
                        title: "Error occured",
                        statusCode: StatusCodes.Status400BadRequest,
                        extensions: new Dictionary<string, object?>
                        {
                            { "errors", new[] { result.Error } }
                        }
                    ));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCity(CityDTO cityDTO)
        {
            CityEntity cityEntity = new CityEntity()
            {
                Id = cityDTO.id,
                CityName = cityDTO.cityName,
                CityPopulation = cityDTO.cityPopulation,
                CountryId = cityDTO.countryId
            };

            Result<CityEntity> result = await _repository.GetAsync(cityEntity.Id);
            CityEntity? previousCity = result.IsSuccess ? result.Value : null;

            if (previousCity != null)
            {
                Result<bool> updateResult = await _repository.PutAsync(cityEntity);
                bool updated = updateResult.IsSuccess ? updateResult.Value : false;

                if (updated)
                {
                    _publishEndpoint.Publish(new CityUpdated(cityEntity.Id, cityEntity.CityName, previousCity.CountryId, cityEntity.CountryId, cityEntity.CityPopulation));
                    return Ok();
                }
                else
                {
                    return new ObjectResult(Results.Problem(
                            title: "Error occured",
                            statusCode: StatusCodes.Status400BadRequest,
                            extensions: new Dictionary<string, object?>
                            {
                            { "errors", new[] { updateResult.Error } }
                            }
                        ));
                }
            }
            else
            {
                return new ObjectResult(Results.Problem(
                            title: "Error occured",
                            statusCode: StatusCodes.Status400BadRequest,
                            extensions: new Dictionary<string, object?>
                            {
                            { "errors", new[] { result.Error } }
                            }
                        ));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCity(string cityName)
        {
            Result<CityEntity> result = await _repository.GetAsync(entity => entity.CityName == cityName);
            CityEntity? cityEntity = result.IsSuccess ? result.Value : null;

            if (cityEntity != null)
            {
                Result<bool> deleteResult = await _repository.DeleteAsync(cityEntity.Id);
                bool deleted = deleteResult.IsSuccess ? deleteResult.Value : false;

                if (deleted)
                {
                    _publishEndpoint.Publish(new CityDeleted(cityEntity.Id, cityEntity.CountryId));
                    return Ok();
                }
                else
                {
                    return new ObjectResult(Results.Problem(
                            title: "Error occured",
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
                            title: "Error occured",
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
