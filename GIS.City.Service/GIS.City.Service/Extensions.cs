﻿using GIS.City.Service.DTOs;
using GIS.City.Service.Entities;

namespace GIS.City.Service
{
    public static class Extensions
    {
        public static CityDTO AsCityDTO(this CityEntity entity)
        {
            return new CityDTO(entity.Id, entity.CityName, entity.CountryId, entity.CityPopulation, entity.Districts.Select(district => district.AsDistrictDTO()).ToList());
        }

        public static DistrictDTO AsDistrictDTO(this DistrictEntity entity)
        {
            return new DistrictDTO(entity.Name, entity.Population);
        }

        public static GIS.DistrictContracts.District AsDistrict(this DistrictEntity districtEntity)
        {
            return new DistrictContracts.District(districtEntity.Name, districtEntity.Population);
        }
    }
}
