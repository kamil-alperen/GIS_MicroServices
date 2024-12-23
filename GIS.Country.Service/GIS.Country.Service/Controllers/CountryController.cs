using System.Diagnostics.Metrics;
using GIS.Common;
using GIS.Common.Repositories;
using GIS.Country.Service.DTOs;
using GIS.Country.Service.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GIS.Country.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CountryController : Controller
    {
        private readonly ILogger<CountryController> _logger;
        private readonly IRepository<CountryEntity> repository;

        public CountryController(ILogger<CountryController> logger, IRepository<CountryEntity> repository)
        {
            _logger = logger;
            this.repository = repository;
        }

        [HttpGet("GetAll", Name = "GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            Result<IReadOnlyCollection<CountryEntity>> result = await repository.GetAllAsync();

            return result.IsSuccess ? Ok(result.Value?.Select(ent => ent.CountryAsDto()).ToList()) : new ObjectResult(Results.Problem(
                    title: "Error occured",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        { "errors", new[] { result.Error } }
                    }
                ));
        }

        [HttpGet("Get", Name = "Get")]
        public async Task<IActionResult> GetAsync([FromQuery] Guid id)
        {
            Result<CountryEntity> result = await repository.GetAsync(id);
            return result.IsSuccess ? Ok(result.Value?.CountryAsDto()) : new ObjectResult(Results.Problem(
                    title: "Error occured",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        { "errors", new[] { result.Error } }
                    }
                ));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromBody] CountryCreateDTO countryCreateDTO)
        {
            CountryEntity countryEntity = new CountryEntity()
            {
                Id = countryCreateDTO.id,
                Name = countryCreateDTO.name,
                Population = countryCreateDTO.population
            };

            Result<CountryEntity> result = await repository.PostAsync(countryEntity);

            return result.IsSuccess ? CreatedAtRoute("Get", new { id = countryCreateDTO.id }, result.Value) : new ObjectResult(Results.Problem(
                    title: "Error occured",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        { "errors", new[] { result.Error } }
                    }
                ));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCountry([FromBody] CountryUpdateDTO countryUpdateDTO)
        {
            CountryEntity countryEntity = new CountryEntity()
            {
                Id = countryUpdateDTO.id,
                Name = countryUpdateDTO.name,
                Population = countryUpdateDTO.population
            };

            Result<bool> result = await repository.PutAsync(countryEntity);

            return result.IsSuccess ? Ok(result.Value) : new ObjectResult(Results.Problem(
                    title: "Error occured",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        { "errors", new[] { result.Error } }
                    }
                ));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCountry([FromQuery] Guid id)
        {
            Result<bool> result = await repository.DeleteAsync(id);

            return result.IsSuccess ? Ok(result.Value) : new ObjectResult(Results.Problem(
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
