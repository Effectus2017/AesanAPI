using Api.Models;
using Api.Services;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con School
/// Contiene todos los métodos de mapeo para DTOSchool y objetos relacionados
/// </summary>
public class SchoolMapper(MappingService mappingService)
{
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Mapea un resultado dinámico a un DTOSchool completo con todas las relaciones
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOSchool</returns>
    public DTOSchool MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOSchool
            {
                Id = item.Id ?? 0,
                AgencyId = item.AgencyId ?? 0,
                Name = item.Name ?? string.Empty,
                SiteCode = item.SiteCode ?? string.Empty,
                StartDate = item.StartDate,
                Address = item.Address ?? string.Empty,
                CityId = item.CityId ?? 0,
                RegionId = item.RegionId ?? 0,
                ZipCode = item.ZipCode ?? string.Empty,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                PostalAddress = item.PostalAddress ?? string.Empty,
                PostalCityId = item.PostalCityId,
                PostalRegionId = item.PostalRegionId,
                PostalZipCode = item.PostalZipCode ?? string.Empty,
                SameAsPhysicalAddress = item.SameAsPhysicalAddress ?? false,
                OrganizationTypeId = item.OrganizationTypeId ?? 0,
                CenterTypeId = item.CenterTypeId,
                NonProfit = item.NonProfit ?? false,
                BaseYear = item.BaseYear,
                RenewalYear = item.RenewalYear,
                OperatingFromDate = item.OperatingFromDate,
                OperatingToDate = item.OperatingToDate,
                OperatingDaysCalculated = item.OperatingDaysCalculated,
                KitchenTypeId = item.KitchenTypeId,
                GroupTypeId = item.GroupTypeId,
                DeliveryTypeId = item.DeliveryTypeId,
                SponsorTypeId = item.SponsorTypeId,
                ApplicantTypeId = item.ApplicantTypeId,
                ResidentialTypeId = item.ResidentialTypeId,
                OperatingPolicyId = item.OperatingPolicyId,
                AreaTypeId = item.AreaTypeId,
                HasWarehouse = item.HasWarehouse ?? false,
                HasDiningRoom = item.HasDiningRoom ?? false,
                AdministratorAuthorizedName = item.AdministratorAuthorizedName ?? string.Empty,
                SitePhone = item.SitePhone ?? string.Empty,
                Extension = item.Extension ?? string.Empty,
                MobilePhone = item.MobilePhone ?? string.Empty,
                Breakfast = item.Breakfast ?? false,
                BreakfastFrom = item.BreakfastFrom,
                BreakfastTo = item.BreakfastTo,
                Lunch = item.Lunch ?? false,
                LunchFrom = item.LunchFrom,
                LunchTo = item.LunchTo,
                Snack = item.Snack ?? false,
                SnackFrom = item.SnackFrom,
                SnackTo = item.SnackTo,
                Dinner = item.Dinner ?? false,
                DinnerFrom = item.DinnerFrom,
                DinnerTo = item.DinnerTo,
                SnackNight = item.SnackNight ?? false,
                SnackNightFrom = item.SnackNightFrom,
                SnackNightTo = item.SnackNightTo,
                CommunityId = item.CommunityId,
                WalkersId = item.WalkersId,
                SiteTypeId = item.SiteTypeId,
                ExperienceId = item.ExperienceId,
                ReviewResultId = item.ReviewResultId,
                ReviewDate = item.ReviewDate,
                ReviewJustification = item.ReviewJustification ?? string.Empty,
                IsActive = item.IsActive ?? true,
                InactiveJustification = item.InactiveJustification ?? string.Empty,
                InactiveDate = item.InactiveDate,
                CreatedAt = item.CreatedAt ?? DateTime.MinValue,
                UpdatedAt = item.UpdatedAt ?? DateTime.MinValue,
                IsMainSchool = item.IsMainSchool ?? false,
                MainSchoolId = item.MainSchoolId ?? 0,
                MainSchoolName = item.MainSchoolName ?? string.Empty,

                // Nested catalogs
                City = item.CityId != null ? _mappingService.MapCity(new { Id = item.CityId, Name = item.CityName ?? string.Empty }) : null,
                Region = item.RegionId != null ? _mappingService.MapRegion(new { Id = item.RegionId, Name = item.RegionName ?? string.Empty }) : null,
                PostalCity = item.PostalCityId != null ? _mappingService.MapCity(new { Id = item.PostalCityId, Name = item.PostalCityName ?? string.Empty }) : null,
                PostalRegion = item.PostalRegionId != null ? _mappingService.MapRegion(new { Id = item.PostalRegionId, Name = item.PostalRegionName ?? string.Empty }) : null,
                OrganizationType = _mappingService.MapOrganizationType(item.OrganizationTypeId, item.OrganizationTypeName, item.OrganizationTypeNameEN),
                KitchenType = _mappingService.MapKitchenType(item.KitchenTypeId, item.KitchenTypeName, item.KitchenTypeNameEN),
                GroupType = _mappingService.MapGroupType(item.GroupTypeId, item.GroupTypeName, item.GroupTypeNameEN),
                DeliveryType = _mappingService.MapDeliveryType(item.DeliveryTypeId, item.DeliveryTypeName, item.DeliveryTypeNameEN),
                SponsorType = _mappingService.MapSponsorType(item.SponsorTypeId, item.SponsorTypeName, item.SponsorTypeNameEN),
                ApplicantType = _mappingService.MapApplicantType(item.ApplicantTypeId, item.ApplicantTypeName, item.ApplicantTypeNameEN),
                ResidentialType = _mappingService.MapResidentialType(item.ResidentialTypeId, item.ResidentialTypeName, item.ResidentialTypeNameEN),
                OperatingPolicy = _mappingService.MapOperatingPolicy(item.OperatingPolicyId, item.OperatingPolicyName, item.OperatingPolicyNameEN),
                CenterType = _mappingService.MapCenterType(item.CenterTypeId, item.CenterName, item.CenterNameEN),
                AreaType = _mappingService.MapAreaType(item.AreaTypeId, item.AreaTypeName, item.AreaTypeNameEN),
                Agency = item.AgencyId != null ? new DTOAgency
                {
                    Id = item.AgencyId,
                    Name = item.AgencyName ?? string.Empty
                } : null,
                MainSchool = item.MainSchoolId != null ? new DTOSchool
                {
                    Id = item.MainSchoolId,
                    Name = item.MainSchoolName ?? string.Empty
                } : null
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la escuela: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la escuela: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un DTOSchool simplificado para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOSchool</returns>
    public static DTOSchool MapListFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOSchool
            {
                Id = item.Id ?? 0,
                AgencyId = item.AgencyId ?? 0,
                Name = item.Name ?? string.Empty,
                IsMainSchool = item.IsMainSchool ?? false,
                MainSchoolId = item.MainSchoolId ?? 0,
                MainSchool = item.MainSchoolId != null ? new DTOSchool
                {
                    Id = item.MainSchoolId,
                    Name = item.MainSchoolName ?? string.Empty
                } : null
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la escuela: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la escuela: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un DTOFacility
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOFacility</returns>
    public static DTOFacility MapFacilityFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                return null;
            }

            return new DTOFacility
            {
                Id = item.Id,
                Name = item.Name
            };
        }
        catch (Exception ex)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un DTOSatelliteSchool
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOSatelliteSchool</returns>
    public static DTOSatelliteSchool MapSatelliteFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                return null;
            }

            return new DTOSatelliteSchool
            {
                Id = item.Id,
                MainSchoolId = item.MainSchoolId,
                SatelliteSchoolId = item.SatelliteSchoolId,
                SatelliteSchoolName = item.SatelliteSchoolName,
                AssignmentDate = item.AssignmentDate,
                Comment = item.Comment,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un DTOEducationLevel
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOEducationLevel</returns>
    public static DTOEducationLevel MapEducationLevelFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                return null;
            }

            return new DTOEducationLevel
            {
                Id = item.Id,
                Name = item.Name ?? string.Empty,
                NameEN = item.NameEN ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }
}
