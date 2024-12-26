using GIS.Country.Service.DTOs;
using GIS.Country.Service.Entities;

namespace GIS.Country.Service
{
    public static class Extensions
    {
        public static CountryDTO CountryAsDto(this CountryEntity countryEntity)
        {
            return new CountryDTO(countryEntity.Id, countryEntity.Name, countryEntity.Population, countryEntity.Cities !=null ? countryEntity.Cities.Select(city => city.CityAsDto()).ToList() : null);
        }

        public static GIS.CountryContracts.Country AsCountry(this CountryEntity countryEntity)
        {
            return new GIS.CountryContracts.Country(countryEntity.Id);
        }

        public static CityDTO CityAsDto(this CityEntity cityEntity) 
        {
            return new CityDTO(cityEntity.Name, cityEntity.Population, cityEntity.Districts);
        }
    }
}
