using System.ComponentModel.DataAnnotations;

namespace GIS.Country.Service.DTOs
{
    public record CountryDTO([Required] Guid id, [Required] string name, long population, [Required] List<CityDTO> cities);
    public record CountryCreateDTO([Required] Guid id, [Required] string name, long population);
    public record CountryUpdateDTO([Required] Guid id, string name, long population);
}