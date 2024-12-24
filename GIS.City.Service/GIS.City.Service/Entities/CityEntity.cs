using System.ComponentModel.DataAnnotations;
using GIS.Common;

namespace GIS.City.Service.Entities
{
    public class CityEntity : IEntity
    {
        public Guid Id { get; set; }
        public string CityName { get; set; }
        public Guid CountryId { get; set; }
        public long CityPopulation { get; set; }
        public List<DistrictEntity> Districts { get; set; }
    }
}
