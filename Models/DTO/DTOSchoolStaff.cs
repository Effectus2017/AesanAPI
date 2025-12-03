namespace Api.Models;

/// <summary>
/// DTO que representa la relación entre un sitio y un empleado del staff
/// Incluye información completa del staff, sitio y asignación
/// </summary>
public class DTOSchoolStaff
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int StaffId { get; set; }
    public DateTime AssignmentDate { get; set; }
    public int AssignmentTypeId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Información del Staff
    public string StaffFirstName { get; set; } = "";
    public string? StaffMiddleName { get; set; }
    public string StaffFatherLastName { get; set; } = "";
    public string StaffMotherLastName { get; set; } = "";
    public string StaffEmail { get; set; } = "";
    public int StaffPositionId { get; set; }
    public int StaffTypeId { get; set; }
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
    public DateTime BirthDate { get; set; }
    public string PostalAddress { get; set; } = "";
    public int StaffCityId { get; set; }
    public int StaffRegionId { get; set; }
    public string StaffZipCode { get; set; } = "";
    public string? StaffComments { get; set; }
    public string? StaffUserId { get; set; }
    public bool StaffIsActive { get; set; }

    // Información del Sitio
    public string SchoolName { get; set; } = "";
    public string Address { get; set; } = "";
    public int SchoolCityId { get; set; }
    public int SchoolRegionId { get; set; }
    public string SchoolZipCode { get; set; } = "";
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? PostalCityId { get; set; }
    public int? PostalRegionId { get; set; }
    public string? PostalZipCode { get; set; }
    public bool? SameAsPhysicalAddress { get; set; }
    public int OrganizationTypeId { get; set; }
    public int? CenterTypeId { get; set; }
    public bool? NonProfit { get; set; }
    public int? BaseYear { get; set; }
    public int? RenewalYear { get; set; }
    public DateTime? OperatingFromDate { get; set; }
    public DateTime? OperatingToDate { get; set; }
    public int? OperatingDaysCalculated { get; set; }
    public int? KitchenTypeId { get; set; }
    public int? GroupTypeId { get; set; }
    public int? DeliveryTypeId { get; set; }
    public int? SponsorTypeId { get; set; }
    public int? ApplicantTypeId { get; set; }
    public int? ResidentialTypeId { get; set; }
    public int? OperatingPolicyId { get; set; }
    public int? AreaTypeId { get; set; }
    public bool? HasWarehouse { get; set; }
    public bool? HasDiningRoom { get; set; }
    public string? AdministratorAuthorizedName { get; set; }
    public string? SitePhone { get; set; }
    public string? Extension { get; set; }
    public string? MobilePhone { get; set; }
    public bool? Breakfast { get; set; }
    public TimeSpan? BreakfastFrom { get; set; }
    public TimeSpan? BreakfastTo { get; set; }
    public bool? Lunch { get; set; }
    public TimeSpan? LunchFrom { get; set; }
    public TimeSpan? LunchTo { get; set; }
    public bool? Snack { get; set; }
    public TimeSpan? SnackFrom { get; set; }
    public TimeSpan? SnackTo { get; set; }
    public bool? Dinner { get; set; }
    public TimeSpan? DinnerFrom { get; set; }
    public TimeSpan? DinnerTo { get; set; }
    public bool? SnackNight { get; set; }
    public TimeSpan? SnackNightFrom { get; set; }
    public TimeSpan? SnackNightTo { get; set; }
    public int? CommunityId { get; set; }
    public int? WalkersId { get; set; }
    public int? SiteTypeId { get; set; }
    public int? ExperienceId { get; set; }
    public int? ReviewResultId { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewJustification { get; set; }
    public bool SchoolIsActive { get; set; }
    public string? InactiveJustification { get; set; }
    public DateTime? InactiveDate { get; set; }
    public DateTime SchoolCreatedAt { get; set; }
    public DateTime? SchoolUpdatedAt { get; set; }

    // Información de la Asignación
    public string AssignmentTypeName { get; set; } = "";
    public string AssignmentTypeNameEn { get; set; } = "";

    // Información de la posición del staff
    public string StaffPositionName { get; set; } = "";
    public string StaffPositionNameEn { get; set; } = "";

    // Información del tipo de staff
    public string StaffTypeName { get; set; } = "";
    public string StaffTypeNameEn { get; set; } = "";

    // Información de la ciudad del staff
    public string StaffCityName { get; set; } = "";
    public string StaffCityNameEn { get; set; } = "";

    // Información de la región del staff
    public string StaffRegionName { get; set; } = "";
    public string StaffRegionNameEn { get; set; } = "";

    // Información de la agencia del sitio
    public string AgencyName { get; set; } = "";
    public bool AgencyIsActive { get; set; }
}
