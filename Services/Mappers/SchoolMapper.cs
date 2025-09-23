using Api.Models;
using Api.Models.Response;
using Api.Services;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con School
/// Contiene todos los métodos de mapeo para DTOSchool y objetos relacionados
/// </summary>
public class SchoolMapper(Lazy<MappingService> mappingService)
{
    private readonly Lazy<MappingService> _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

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
                LocationTypeId = item.LocationTypeId,
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
                GeneralEnrollment = item.GeneralEnrollment,

                // Nested catalogs
                City = item.CityId != null ? _mappingService.Value.MapCity(new { Id = item.CityId, Name = item.CityName ?? string.Empty }) : null,
                Region = item.RegionId != null ? _mappingService.Value.MapRegion(new { Id = item.RegionId, Name = item.RegionName ?? string.Empty }) : null,
                PostalCity = item.PostalCityId != null ? _mappingService.Value.MapCity(new { Id = item.PostalCityId, Name = item.PostalCityName ?? string.Empty }) : null,
                PostalRegion = item.PostalRegionId != null ? _mappingService.Value.MapRegion(new { Id = item.PostalRegionId, Name = item.PostalRegionName ?? string.Empty }) : null,
                OrganizationType = _mappingService.Value.MapOrganizationType(item.OrganizationTypeId, item.OrganizationTypeName, item.OrganizationTypeNameEN),
                KitchenType = _mappingService.Value.MapKitchenType(item.KitchenTypeId, item.KitchenTypeName, item.KitchenTypeNameEN),
                GroupType = _mappingService.Value.MapGroupType(item.GroupTypeId, item.GroupTypeName, item.GroupTypeNameEN),
                DeliveryType = _mappingService.Value.MapDeliveryType(item.DeliveryTypeId, item.DeliveryTypeName, item.DeliveryTypeNameEN),
                SponsorType = _mappingService.Value.MapSponsorType(item.SponsorTypeId, item.SponsorTypeName, item.SponsorTypeNameEN),
                ApplicantType = _mappingService.Value.MapApplicantType(item.ApplicantTypeId, item.ApplicantTypeName, item.ApplicantTypeNameEN),
                ResidentialType = _mappingService.Value.MapResidentialType(item.ResidentialTypeId, item.ResidentialTypeName, item.ResidentialTypeNameEN),
                OperatingPolicy = _mappingService.Value.MapOperatingPolicy(item.OperatingPolicyId, item.OperatingPolicyName, item.OperatingPolicyNameEN),
                CenterType = _mappingService.Value.MapCenterType(item.CenterTypeId, item.CenterName, item.CenterNameEN),
                AreaType = _mappingService.Value.MapAreaType(item.AreaTypeId, item.AreaTypeName, item.AreaTypeNameEN),
                LocationType = _mappingService.Value.MapAreaType(item.LocationTypeId, item.LocationTypeName, item.LocationTypeNameEN),
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
    /// Mapea un resultado dinámico a un SchoolResponse (sin IDs redundantes)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolResponse</returns>
    public SchoolResponse MapResponseFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new SchoolResponse
            {
                Id = item.Id ?? 0,
                AgencyId = item.AgencyId ?? 0,
                Name = item.Name ?? string.Empty,
                SiteCode = item.SiteCode ?? string.Empty,
                StartDate = item.StartDate,
                Address = item.Address ?? string.Empty,
                ZipCode = item.ZipCode ?? string.Empty,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                PostalAddress = item.PostalAddress ?? string.Empty,
                PostalZipCode = item.PostalZipCode ?? string.Empty,
                SameAsPhysicalAddress = item.SameAsPhysicalAddress ?? false,
                NonProfit = item.NonProfit ?? false,
                BaseYear = item.BaseYear,
                RenewalYear = item.RenewalYear,
                OperatingFromDate = item.OperatingFromDate,
                OperatingToDate = item.OperatingToDate,
                OperatingDaysCalculated = item.OperatingDaysCalculated,
                HasWarehouse = item.HasWarehouse ?? false,
                HasDiningRoom = item.HasDiningRoom ?? false,
                AdministratorAuthorizedName = item.AdministratorAuthorizedName ?? string.Empty,
                SitePhone = item.SitePhone ?? string.Empty,
                Extension = item.Extension ?? string.Empty,
                MobilePhone = item.MobilePhone ?? string.Empty,
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
                GeneralEnrollment = item.GeneralEnrollment,

                // Objetos relacionados (sin IDs redundantes)
                City = item.CityId != null ? _mappingService.Value.MapCity(new { Id = item.CityId, Name = item.CityName ?? string.Empty }) : null,
                Region = item.RegionId != null ? _mappingService.Value.MapRegion(new { Id = item.RegionId, Name = item.RegionName ?? string.Empty }) : null,
                PostalCity = item.PostalCityId != null ? _mappingService.Value.MapCity(new { Id = item.PostalCityId, Name = item.PostalCityName ?? string.Empty }) : null,
                PostalRegion = item.PostalRegionId != null ? _mappingService.Value.MapRegion(new { Id = item.PostalRegionId, Name = item.PostalRegionName ?? string.Empty }) : null,
                OrganizationType = _mappingService.Value.MapOrganizationType(item.OrganizationTypeId, item.OrganizationTypeName, item.OrganizationTypeNameEN),
                KitchenType = _mappingService.Value.MapKitchenType(item.KitchenTypeId, item.KitchenTypeName, item.KitchenTypeNameEN),
                GroupType = _mappingService.Value.MapGroupType(item.GroupTypeId, item.GroupTypeName, item.GroupTypeNameEN),
                DeliveryType = _mappingService.Value.MapDeliveryType(item.DeliveryTypeId, item.DeliveryTypeName, item.DeliveryTypeNameEN),
                SponsorType = _mappingService.Value.MapSponsorType(item.SponsorTypeId, item.SponsorTypeName, item.SponsorTypeNameEN),
                ApplicantType = _mappingService.Value.MapApplicantType(item.ApplicantTypeId, item.ApplicantTypeName, item.ApplicantTypeNameEN),
                ResidentialType = _mappingService.Value.MapResidentialType(item.ResidentialTypeId, item.ResidentialTypeName, item.ResidentialTypeNameEN),
                OperatingPolicy = _mappingService.Value.MapOperatingPolicy(item.OperatingPolicyId, item.OperatingPolicyName, item.OperatingPolicyNameEN),
                CenterType = _mappingService.Value.MapCenterType(item.CenterTypeId, item.CenterName, item.CenterNameEN),
                AreaType = _mappingService.Value.MapAreaType(item.AreaTypeId, item.AreaTypeName, item.AreaTypeNameEN),
                LocationType = _mappingService.Value.MapAreaType(item.LocationTypeId, item.LocationTypeName, item.LocationTypeNameEN),
                Agency = item.AgencyId != null ? new DTOAgency
                {
                    Id = item.AgencyId,
                    Name = item.AgencyName ?? string.Empty
                } : null,
                MainSchool = item.MainSchoolId != null ? new SchoolResponse
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
    /// Mapea un resultado dinámico a un SchoolResponse simplificado para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolResponse</returns>
    public static SchoolResponse MapResponseListFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new SchoolResponse
            {
                Id = item.Id ?? 0,
                AgencyId = item.AgencyId ?? 0,
                Name = item.Name ?? string.Empty,
                IsMainSchool = item.IsMainSchool ?? false,
                MainSchoolId = item.MainSchoolId ?? 0,
                MainSchool = item.MainSchoolId != null ? new SchoolResponse
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
    /// Mapea un resultado dinámico a un SchoolTableResponse optimizado para tablas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolTableResponse</returns>
    public static SchoolTableResponse MapTableResponseFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new SchoolTableResponse
            {
                Id = item.Id ?? 0,
                Name = item.Name ?? string.Empty,
                Address = item.Address ?? string.Empty,
                CityName = item.CityName ?? string.Empty,
                RegionName = item.RegionName ?? string.Empty,
                IsMainSchool = item.IsMainSchool ?? false,
                MainSchoolName = item.MainSchoolName,
                GeneralEnrollment = item.GeneralEnrollment,
                SiteNumber = item.SiteNumber ?? 0,
                AgencyCode = item.AgencyCode,
                SiteCode = item.SiteCode
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la escuela para tabla: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la escuela para tabla: {ex.Message}", ex);
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
    /// Mapea un resultado dinámico a un SchoolSatelliteResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolSatelliteResponse</returns>
    public static SchoolSatelliteResponse MapSatelliteFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                return null;
            }

            return new SchoolSatelliteResponse
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

    /// <summary>
    /// Mapea un servicio de escuela desde un resultado dinámico a un SchoolServiceResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolServiceResponse</returns>
    public static SchoolServiceResponse MapSchoolServiceFromResult(dynamic item)
    {
        try
        {
            return new SchoolServiceResponse
            {
                Id = item.Id,
                SchoolId = item.SchoolId,
                ChildGroup = item.ChildGroupId != null ? new SchoolChildGroupResponse
                {
                    Id = item.ChildGroupId,
                    SchoolId = item.SchoolId,
                    GroupName = item.ChildGroupName,
                    NumberOfChildren = item.ChildGroupNumberOfChildren ?? 0,
                    CreatedAt = item.ChildGroupCreatedAt ?? DateTime.MinValue,
                    UpdatedAt = item.ChildGroupUpdatedAt
                } : null,
                Breakfast = item.Breakfast,
                BreakfastFrom = item.BreakfastFrom,
                BreakfastTo = item.BreakfastTo,
                Lunch = item.Lunch,
                LunchFrom = item.LunchFrom,
                LunchTo = item.LunchTo,
                SnackAM = item.SnackAM,
                SnackAMFrom = item.SnackAMFrom,
                SnackAMTo = item.SnackAMTo,
                Dinner = item.Dinner,
                DinnerFrom = item.DinnerFrom,
                DinnerTo = item.DinnerTo,
                SnackPM = item.SnackPM,
                SnackPMFrom = item.SnackPMFrom,
                SnackPMTo = item.SnackPMTo,
                SnackNight = item.SnackNight,
                SnackNightFrom = item.SnackNightFrom,
                SnackNightTo = item.SnackNightTo,
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
    /// Mapea información de Day Care Home desde un resultado dinámico a un SchoolDayCareHomeResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolDayCareHomeResponse</returns>
    public static SchoolDayCareHomeResponse MapSchoolDayCareHomeFromResult(dynamic item)
    {
        try
        {
            return new SchoolDayCareHomeResponse
            {
                Id = item.Id,
                SchoolId = item.SchoolId,
                IsAuthorizedToOperate = item.IsAuthorizedToOperate,
                HasFamilyDepartmentLicense = item.HasFamilyDepartmentLicense,
                NumberOfEnrolledChildren = item.NumberOfEnrolledChildren,
                NumberOfProviderChildren = item.NumberOfProviderChildren,
                NumberOfParticipantsWithBloodTies = item.NumberOfParticipantsWithBloodTies,
                NumberOfParticipantsWithoutBloodTies = item.NumberOfParticipantsWithoutBloodTies,
                MinorsLiveWithProvider = item.MinorsLiveWithProvider,
                RelationshipType = item.RelationshipTypeId != null ? new DTOOptionSelection
                {
                    Id = item.RelationshipTypeId,
                    Name = item.RelationshipTypeName,
                    NameEN = item.RelationshipTypeNameEN,
                    OptionKey = item.RelationshipTypeOptionKey
                } : null,
                OffersServiceToImmigrantChildren = item.OffersServiceToImmigrantChildren,
                HomeType = item.HomeTypeId != null ? new DTOOptionSelection
                {
                    Id = item.HomeTypeId,
                    Name = item.HomeTypeName,
                    NameEN = item.HomeTypeNameEN,
                    OptionKey = item.HomeTypeOptionKey
                } : null,
                AdministratorAuthorizedName = item.AdministratorAuthorizedName,
                AdministratorBirthDate = item.AdministratorBirthDate,
                OffersServiceToDifferentGroups = item.OffersServiceToDifferentGroups,
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
    /// Mapea un participante de escuela desde un resultado dinámico a un SchoolParticipantResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolParticipantResponse</returns>
    public static SchoolParticipantResponse MapSchoolParticipantFromResult(dynamic item)
    {
        try
        {
            return new SchoolParticipantResponse
            {
                Id = item.Id,
                SchoolId = item.SchoolId,
                ParticipantType = new DTOOptionSelection
                {
                    Id = item.ParticipantTypeId,
                    Name = item.ParticipantTypeName,
                    NameEN = item.ParticipantTypeNameEN,
                    OptionKey = item.ParticipantTypeOptionKey
                },
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
    /// Mapea un grupo de niños específico desde un resultado dinámico a un SchoolChildGroupResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SchoolChildGroupResponse</returns>
    public static SchoolChildGroupResponse MapSchoolChildGroupFromResult(dynamic item)
    {
        try
        {
            return new SchoolChildGroupResponse
            {
                Id = item.Id,
                SchoolId = item.SchoolId,
                GroupName = item.GroupName,
                NumberOfChildren = item.NumberOfChildren,
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
}
