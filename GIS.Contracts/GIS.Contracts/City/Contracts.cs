using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIS.Contracts.City
{
    public record CityCreated([Required] Guid id, [Required] string cityName, [Required] Guid countryId, long cityPopulation);
    public record CityUpdated([Required] Guid id, string cityName, Guid previousCountryId, Guid countryId, long cityPopulation);
    public record CityDeleted([Required] Guid id, [Required] Guid countryId);
}
