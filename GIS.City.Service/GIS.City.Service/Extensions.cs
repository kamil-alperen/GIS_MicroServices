using GIS.City.Service.DTOs;
using GIS.City.Service.Entities;

namespace GIS.City.Service
{
    public static class Extensions
    {
        public static CityDTO AsCityDTO(this CityEntity entity)
        {
            return new CityDTO(entity.Id, entity.CityName, entity.CountryId, entity.CityPopulation, entity.Districts != null ? entity.Districts.Select(district => district.AsDistrictDTO()).ToList() : null);
        }

        public static CityContracts.City AsCity(this CityEntity entity)
        {
            return new CityContracts.City(entity.Id);
        }

        public static DistrictDTO AsDistrictDTO(this DistrictEntity entity)
        {
            return new DistrictDTO(entity.Name, entity.Population);
        }

        public static DistrictContracts.DistrictItem AsDistrictItem(this DistrictEntity districtEntity)
        {
            return new DistrictContracts.DistrictItem(districtEntity.Name, districtEntity.Population);
        }
    }
}
