using GIS.Common;

namespace GIS.Country.Service.Entities
{
    public class CityEntity : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public long Population { get; set; }

        public List<DistrictContracts.DistrictItem> Districts { get; set; }
    }
}
