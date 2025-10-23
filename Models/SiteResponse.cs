using Api.Models;
using Api.Models.Response;
using System;
using System.Collections.Generic;

namespace Api.Models;

/// <summary>
/// Modelo de respuesta para Site sin IDs redundantes
/// Contiene solo los objetos relacionados para evitar duplicación de datos
/// </summary>
public class SiteResponse
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

    /// <summary>
    /// ¿Desde cuándo su Entidad ofrece servicios con una matrícula establecida?
    /// Since when has your Entity offered services with an established registration?
    /// </summary>
    public DateTime? ServiceTime { get; set; }

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
    public OrganizationTypeResponse? OrganizationType { get; set; }
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

    public bool IsActive { get; set; }
    public string? InactiveJustification { get; set; }
    public DateTime? InactiveDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? GeneralEnrollment { get; set; }
    public int SiteNumber { get; set; }
    public string? AgencyCode { get; set; }

    // School relacionada
    public SchoolResponse? School { get; set; }

    // Niveles educativos
    public List<EducationLevelResponse>? EducationLevels { get; set; }

    // Servicios de alimentación
    public List<SiteServiceResponse>? Services { get; set; }

    // Información específica de Day Care Home
    public SiteDayCareHomeResponse? DayCareHome { get; set; }

    // Tipos de participantes
    public List<SiteParticipantResponse>? Participants { get; set; }

    // Grupos de niños específicos
    public List<SiteChildGroupResponse>? ChildGroups { get; set; }

    // ===== CAMPOS ESPECÍFICOS PARA PACNA =====

    /// <summary>
    /// ¿El sitio ofrece programas atléticos organizados que participan en deportes competitivos interestelares o a nivel comunitario?
    /// Does the site offer organized athletic programs engaged in interscholastic or community level competitive sports?
    /// Solo para programa PACNA
    /// </summary>
    public bool? OrganizedAthleticPrograms { get; set; }

    /// <summary>
    /// ¿El sitio está interesado en participar en el servicio de merienda y cena en riesgo?
    /// Is the site interested in participating in the at-risk snack and dinner service?
    /// Solo para programa PACNA
    /// </summary>
    public bool? AtRiskService { get; set; }

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
