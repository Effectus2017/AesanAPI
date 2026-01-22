using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

public class SiteRequest
{
    // ===== CAMPOS PRINCIPALES DE SITE =====
    public int? Id { get; set; }
    public int? AgencyId { get; set; }
    public required string Name { get; set; }
    public string? SiteCode { get; set; }

    // Dirección Física
    public required string Address { get; set; }
    public int CityId { get; set; }
    public int RegionId { get; set; }
    public required string ZipCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Dirección Postal
    public string? PostalAddress { get; set; }
    public int? PostalCityId { get; set; }
    public int? PostalRegionId { get; set; }
    public string? PostalZipCode { get; set; }
    public bool? SameAsPhysicalAddress { get; set; }

    // Información Administrativa
    public int OrganizationTypeId { get; set; }
    public int? CenterTypeId { get; set; }
    public bool? NonProfit { get; set; }
    public DateTime? StartDate { get; set; }
    public int? BaseYear { get; set; }
    public int? RenewalYear { get; set; }
    public List<SiteEducationLevelRequest> EducationLevels { get; set; } = new();

    /// <summary>
    /// Fecha desde cuando inicia el funcionamiento del sitio
    /// </summary>
    [Required(ErrorMessage = "La fecha desde es requerida")]
    public DateTime? OperatingFromDate { get; set; }

    /// <summary>
    /// Fecha hasta cuando finaliza el funcionamiento del sitio
    /// </summary>
    [Required(ErrorMessage = "La fecha hasta es requerida")]
    public DateTime? OperatingToDate { get; set; }

    public int? OperatingDaysCalculated { get; set; }

    /// <summary>
    /// Hora de inicio de funcionamiento del sitio
    /// Operating start time of the site
    /// </summary>
    public TimeSpan? OperatingStartTime { get; set; }

    /// <summary>
    /// Hora de fin de funcionamiento del sitio
    /// Operating end time of the site
    /// </summary>
    public TimeSpan? OperatingEndTime { get; set; }

    /// <summary>
    /// Días de la semana en que opera el sitio (1=Lunes, 2=Martes, ..., 7=Domingo)
    /// Days of the week the site operates (1=Monday, 2=Tuesday, ..., 7=Sunday)
    /// </summary>
    public List<int>? OperatingDaysOfWeek { get; set; }

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
    public int? DiningRoomCapacity { get; set; }

    // ===== RELACIONES CON MODELOS REQUEST =====
    public SitePersonInChargeRequest? PersonInCharge { get; set; }
    public List<SiteServiceRequest> Services { get; set; } = new();
    public SiteDayCareHomeRequest? DayCareHome { get; set; }
    public List<SiteParticipantRequest> Participants { get; set; } = new();
    public List<SiteChildGroupRequest> ChildGroups { get; set; } = new();

    // Campos adicionales
    public int? CommunityId { get; set; }
    public int? WalkersId { get; set; }
    public int? SiteTypeId { get; set; }
    public int? SiteLocationId { get; set; }
    public int? ExperienceId { get; set; }
    public int? ReviewResultId { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewJustification { get; set; }

    // School asignada
    public int? SchoolId { get; set; }

    // Estado de actividad
    public bool? IsActive { get; set; }
    public string? InactiveJustification { get; set; }
    public DateTime? InactiveDate { get; set; }

    // Matrícula General
    public int? GeneralEnrollment { get; set; }

    // Número de Sitio
    public int? SiteNumber { get; set; }

    // ===== CAMPOS ESPECÍFICOS PARA PACNA =====

    /// <summary>
    /// ¿El sitio ofrece programas atléticos organizados que participan en deportes competitivos interestelares o a nivel comunitario?
    /// Does the site offer organized athletic programs engaged in interscholastic or community level competitive sports?
    /// Solo para programa PACNA
    /// </summary>
    public bool? OrganizedAthleticPrograms { get; set; } = false;

    /// <summary>
    /// De poseer un contrato Público Alianza, especifique su modalidad
    /// If you have a Public Alliance contract, please specify the type of contract
    /// Socio-Económico (17), Híbrido (18)
    /// </summary>
    public int? PublicAllianceContractId { get; set; }

    /// <summary>
    /// ¿El sitio está interesado en participar en el servicio de merienda y cena en riesgo?
    /// Is the site interested in participating in the at-risk snack and dinner service?
    /// Solo para programa PACNA
    /// </summary>
    public bool? AtRiskService { get; set; } = false;

    /// <summary>
    /// ¿Es un centro o institución afiliada?
    /// Is it an affiliated center or institution?
    /// Solo para programa PACNA
    /// </summary>
    public bool? IsAffiliatedCenter { get; set; }

    /// <summary>
    /// ID que indica si el sitio es un Centro (No) o un Hogar (Sí)
    /// ID indicating if the site is a Center (No) or a Home (Yes)
    /// Solo para sitios de agencias con programa PACNA
    /// </summary>
    public int? IsDayCareHomeId { get; set; }

    /// <summary>
    /// IDs de programas de la agencia
    /// Agency program IDs
    /// Se obtiene desde el frontend para determinar la lógica de días de funcionamiento
    /// </summary>
    public List<int>? ProgramIds { get; set; }
}
