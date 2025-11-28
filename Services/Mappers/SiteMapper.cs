using Api.Models;
using Api.Models.Response;
using Api.Services;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con Site
/// Contiene todos los métodos de mapeo para SiteResponse y objetos relacionados
/// </summary>
public class SiteMapper(Lazy<MappingService> mappingService)
{
    private readonly Lazy<MappingService> _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Mapea un resultado dinámico a un SiteResponse (sin IDs redundantes)
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteResponse</returns>
    public SiteResponse MapResponseFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new SiteResponse
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
                GeneralEnrollment = item.GeneralEnrollment,

                // ===== CAMPOS ESPECÍFICOS PARA PACNA =====
                OrganizedAthleticPrograms = item.OrganizedAthleticPrograms,
                AtRiskService = item.AtRiskService,
                IsDayCareHomeId = item.IsDayCareHomeId,
                IsDayCareHome = item.IsDayCareHomeId != null ? new DTOOptionSelection
                {
                    Id = item.IsDayCareHomeId,
                    Name = item.IsDayCareHomeName ?? string.Empty,
                    NameEN = item.IsDayCareHomeNameEN ?? string.Empty,
                    OptionKey = item.IsDayCareHomeOptionKey ?? string.Empty,
                    BooleanValue = item.IsDayCareHomeBooleanValue
                } : null,

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
                PersonInCharge = item.PersonInChargeFirstName != null || item.PersonInChargeFatherLastName != null ? new SitePersonInChargeResponse
                {
                    SiteId = item.Id ?? 0,
                    FirstName = item.PersonInChargeFirstName,
                    MiddleName = item.PersonInChargeMiddleName,
                    FatherLastName = item.PersonInChargeFatherLastName,
                    MotherLastName = item.PersonInChargeMotherLastName,
                    SitePhone = item.PersonInChargeSitePhone,
                    Extension = item.PersonInChargeExtension,
                    MobilePhone = item.PersonInChargeMobilePhone
                } : null
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el sitio: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el sitio: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un SiteResponse simplificado para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteResponse</returns>
    public static SiteResponse MapResponseListFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new SiteResponse
            {
                Id = item.Id ?? 0,
                AgencyId = item.AgencyId ?? 0,
                Name = item.Name ?? string.Empty,
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el sitio: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el sitio: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un SiteListItemResponse para elementos de lista simples
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteListItemResponse</returns>
    public static SiteListItemResponse MapListItemFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                return null;
            }

            return new SiteListItemResponse
            {
                Id = item.Id ?? 0,
                Name = item.Name ?? string.Empty
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el sitio: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el sitio: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un SiteTableResponse optimizado para tablas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteTableResponse</returns>
    public static SiteTableResponse MapTableResponseFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new SiteTableResponse
            {
                Id = item.Id ?? 0,
                Name = item.Name ?? string.Empty,
                Address = item.Address ?? string.Empty,
                CityName = item.CityName ?? string.Empty,
                RegionName = item.RegionName ?? string.Empty,
                IsActive = item.IsActive ?? true,
                GroupTypeName = item.GroupTypeName,
                GeneralEnrollment = item.GeneralEnrollment,
                SiteNumber = item.SiteNumber ?? 0,
                AgencyCode = item.AgencyCode,
                SiteCode = item.SiteCode,
                SchoolName = item.SchoolName,
                SchoolId = item.SchoolId
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el sitio para tabla: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el sitio para tabla: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un EducationLevelResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>EducationLevelResponse</returns>
    public static EducationLevelResponse MapEducationLevelFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                return null;
            }

            return new EducationLevelResponse
            {
                Id = item.EducationLevelId, // Usar EducationLevelId (FK a EducationLevel) en lugar de Id (PK de SiteEducationLevel)
                Name = item.EducationLevelName ?? string.Empty,
                NameEN = item.EducationLevelNameEN ?? string.Empty
            };
        }
        catch (Exception)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }

    /// <summary>
    /// Mapea un servicio de sitio desde un resultado dinámico a un SiteServiceResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteServiceResponse</returns>
    public static SiteServiceResponse MapSiteServiceFromResult(dynamic item)
    {
        try
        {
            return new SiteServiceResponse
            {
                Id = item.Id,
                SiteId = item.SiteId,
                ChildGroup = item.ChildGroupId != null ? new SiteChildGroupResponse
                {
                    Id = item.ChildGroupId,
                    SiteId = item.SiteId,
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
        catch (Exception)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }

    /// <summary>
    /// Mapea información de Day Care Home desde un resultado dinámico a un SiteDayCareHomeResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteDayCareHomeResponse</returns>
    public static SiteDayCareHomeResponse MapSiteDayCareHomeFromResult(dynamic item)
    {
        try
        {
            return new SiteDayCareHomeResponse
            {
                Id = item.Id,
                SiteId = item.SiteId,
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
        catch (Exception)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }

    /// <summary>
    /// Mapea un participante de sitio desde un resultado dinámico a un SiteParticipantResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteParticipantResponse</returns>
    public static SiteParticipantResponse MapSiteParticipantFromResult(dynamic item)
    {
        try
        {
            return new SiteParticipantResponse
            {
                Id = item.Id,
                SiteId = item.SiteId,
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
        catch (Exception)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }

    /// <summary>
    /// Mapea un grupo de niños específico desde un resultado dinámico a un SiteChildGroupResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>SiteChildGroupResponse</returns>
    public static SiteChildGroupResponse MapSiteChildGroupFromResult(dynamic item)
    {
        try
        {
            return new SiteChildGroupResponse
            {
                Id = item.Id,
                SiteId = item.SiteId,
                GroupName = item.GroupName,
                NumberOfChildren = item.NumberOfChildren,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }
        catch (Exception)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }
}
