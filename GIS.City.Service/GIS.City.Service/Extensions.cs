using GIS.City.Service.DTOs;
using GIS.City.Service.Entities;

namespace GIS.City.Service
{
    public static class Extensions
    {
        public static CityDTO AsCityDTO(this CityEntity entity)
        {
            return new CityDTO(entity.Id, entity.CityName, entity.CountryId, entity.CityPopulation);
        }
    }
}
