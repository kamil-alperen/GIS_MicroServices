using System.ComponentModel.DataAnnotations;

namespace GIS.City.Service.DTOs
{
    public record CityDTO([Required] Guid id, [Required] string cityName, [Required] Guid countryId, long cityPopulation, List<DistrictDTO> districts);
    public record CityCreateDTO([Required] Guid id, [Required] string cityName, [Required] Guid countryId, long cityPopulation);
    public record CityUpdateDTO([Required] Guid id, string cityName, Guid countryId, long cityPopulation);
}
