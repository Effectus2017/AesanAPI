using Api.Models;
using Api.Models.Response;
using System;
using System.Collections.Generic;

namespace Api.Models;

/// <summary>
/// Modelo de respuesta para School sin IDs redundantes
/// Contiene solo los objetos relacionados para evitar duplicación de datos
/// </summary>
public class SchoolResponse
{
    public int Id { get; set; }
    public int AgencyId { get; set; }
    public string Name { get; set; }
    public string? SiteCode { get; set; }
    public DateTime? StartDate { get; set; }
    public string Address { get; set; }
    public string ZipCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string PostalAddress { get; set; }
    public string PostalZipCode { get; set; }
    public bool? SameAsPhysicalAddress { get; set; }

    // Información Administrativa
    public bool? NonProfit { get; set; }
    public int? BaseYear { get; set; }
    public int? RenewalYear { get; set; }

    // Información Operacional
    public DateTime? OperatingFromDate { get; set; }
    public DateTime? OperatingToDate { get; set; }
    public int? OperatingDaysCalculated { get; set; }
    public bool? HasWarehouse { get; set; }
    public bool? HasDiningRoom { get; set; }

    // Administrador/Representante Autorizado
    public string AdministratorAuthorizedName { get; set; }
    public string SitePhone { get; set; }
    public string Extension { get; set; }
    public string MobilePhone { get; set; }

    // Campos adicionales
    public int? CommunityId { get; set; }
    public int? WalkersId { get; set; }
    public int? SiteTypeId { get; set; }
    public int? ExperienceId { get; set; }
    public int? ReviewResultId { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewJustification { get; set; }

    // Objetos relacionados (sin IDs redundantes)
    public DTOCity? City { get; set; }
    public DTORegion? Region { get; set; }
    public DTOCity? PostalCity { get; set; }
    public DTORegion? PostalRegion { get; set; }

    public List<int> EducationLevelIds { get; set; } = [];
    public DTOOperatingPeriod? OperatingPeriod { get; set; }
    public DTOOrganizationType? OrganizationType { get; set; }
    public DTOKitchenType? KitchenType { get; set; }
    public DTOGroupType? GroupType { get; set; }
    public DeliveryTypeResponse? DeliveryType { get; set; }
    public DTOSponsorType? SponsorType { get; set; }
    public DTOApplicantType? ApplicantType { get; set; }
    public DTOResidentialType? ResidentialType { get; set; }
    public DTOOperatingPolicy? OperatingPolicy { get; set; }
    public DTOCenterType? CenterType { get; set; }
    public DTOAreaType? AreaType { get; set; }
    public DTOAreaType? LocationType { get; set; }
    public DTOAgency? Agency { get; set; }

    public bool IsMainSchool { get; set; }
    public bool IsActive { get; set; }
    public string? InactiveJustification { get; set; }
    public DateTime? InactiveDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? MainSchoolId { get; set; }
    public string? MainSchoolName { get; set; }
    public int? GeneralEnrollment { get; set; }
    public int SiteNumber { get; set; }
    public string? AgencyCode { get; set; }

    // Main School
    public SchoolResponse? MainSchool { get; set; }

    // Satélites
    public List<SchoolSatelliteResponse>? Satellites { get; set; }

    // Niveles educativos
    public List<DTOEducationLevel>? EducationLevels { get; set; }

    // Servicios de alimentación
    public List<SchoolServiceResponse>? Services { get; set; }

    // Información específica de Day Care Home
    public SchoolDayCareHomeResponse? DayCareHome { get; set; }

    // Tipos de participantes
    public List<SchoolParticipantResponse>? Participants { get; set; }

    // Propiedades de conveniencia para operaciones que necesiten IDs
    public int CityId => City?.Id ?? 0;
    public int RegionId => Region?.Id ?? 0;
    public int? PostalCityId => PostalCity?.Id;
    public int? PostalRegionId => PostalRegion?.Id;
    public int OrganizationTypeId => OrganizationType?.Id ?? 0;
    public int? CenterTypeId => CenterType?.Id;
    public int? KitchenTypeId => KitchenType?.Id;
    public int? GroupTypeId => GroupType?.Id;
    public int? DeliveryTypeId => DeliveryType?.Id;
    public int? SponsorTypeId => SponsorType?.Id;
    public int? ApplicantTypeId => ApplicantType?.Id;
    public int? ResidentialTypeId => ResidentialType?.Id;
    public int? OperatingPolicyId => OperatingPolicy?.Id;
    public int? AreaTypeId => AreaType?.Id;
    public int? LocationTypeId => LocationType?.Id;
}
