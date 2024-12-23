using System.ComponentModel.DataAnnotations;

namespace GIS.City.Service.DTOs
{
    public record CityDTO([Required] Guid id, [Required] string cityName, [Required] Guid countryId, long cityPopulation);
}
