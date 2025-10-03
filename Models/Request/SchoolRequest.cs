using System;
using System.Collections.Generic;

namespace Api.Models.Request;

public class SchoolRequest
{
    // ===== CAMPOS PRINCIPALES DE SCHOOL =====
    public int? Id { get; set; }
    public int? AgencyId { get; set; }
    public string Name { get; set; }
    public string? SiteCode { get; set; }

    // Dirección Física
    public string Address { get; set; }
    public int CityId { get; set; }
    public int RegionId { get; set; }
    public string ZipCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Dirección Postal
    public string PostalAddress { get; set; }
    public int? PostalCityId { get; set; }
    public int? PostalRegionId { get; set; }
    public string PostalZipCode { get; set; }
    public bool? SameAsPhysicalAddress { get; set; }

    // Información Administrativa
    public int OrganizationTypeId { get; set; }
    public int? CenterTypeId { get; set; }
    public bool? NonProfit { get; set; }
    public DateTime? StartDate { get; set; }
    public int? BaseYear { get; set; }
    public int? RenewalYear { get; set; }
    public List<SchoolEducationLevelRequest> EducationLevels { get; set; } = new();
    public DateTime? OperatingFromDate { get; set; }
    public DateTime? OperatingToDate { get; set; }
    public int? OperatingDaysCalculated { get; set; }

    /// <summary>
    /// ¿Desde cuándo su Entidad ofrece servicios con una matrícula establecida?
    /// Since when has your Entity offered services with an established registration?
    /// </summary>
    public DateTime? ServiceTime { get; set; }

    // Información Operacional
    public int? KitchenTypeId { get; set; }
    public int? GroupTypeId { get; set; }
    public int? DeliveryTypeId { get; set; }
    public int? SponsorTypeId { get; set; }
    public int? ApplicantTypeId { get; set; }
    public int? ResidentialTypeId { get; set; }
    public int? OperatingPolicyId { get; set; }
    public int? AreaTypeId { get; set; }
    public int? LocationTypeId { get; set; }
    public bool? HasWarehouse { get; set; }
    public bool? HasDiningRoom { get; set; }

    // Administrador/Representante Autorizado
    public string AdministratorAuthorizedName { get; set; }
    public string SitePhone { get; set; }
    public string Extension { get; set; }
    public string MobilePhone { get; set; }

    // ===== RELACIONES CON MODELOS REQUEST =====
    public List<SchoolServiceRequest> Services { get; set; } = new();
    public SchoolDayCareHomeRequest? DayCareHome { get; set; }
    public List<SchoolParticipantRequest> Participants { get; set; } = new();
    public List<SchoolChildGroupRequest> ChildGroups { get; set; } = new();

    // Campos adicionales
    public int? CommunityId { get; set; }
    public int? WalkersId { get; set; }
    public int? SiteTypeId { get; set; }
    public int? ExperienceId { get; set; }
    public int? ReviewResultId { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewJustification { get; set; }

    // Escuela Principal
    public bool? IsMainSchool { get; set; }
    public int? MainSchoolId { get; set; }

    // Estado de actividad
    public bool? IsActive { get; set; }
    public string? InactiveJustification { get; set; }
    public DateTime? InactiveDate { get; set; }

    // Matrícula General
    public int? GeneralEnrollment { get; set; }

    // Número de Sitio
    public int? SiteNumber { get; set; }
}