using GIS.Country.Service.DTOs;
using GIS.Country.Service.Entities;

namespace GIS.Country.Service
{
    public static class Extensions
    {
        public static CountryDTO CountryAsDto(this CountryEntity countryEntity)
        {
            return new CountryDTO(countryEntity.Id, countryEntity.Name, countryEntity.Population, countryEntity.Cities.Select(city => city.CityAsDto()).ToList());
        }

        public static CityDTO CityAsDto(this CityEntity cityEntity) 
        {
            return new CityDTO(cityEntity.Name, cityEntity.Population);
        }
    }
}
