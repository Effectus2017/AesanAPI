using Api.Models;
using System;
using System.Collections.Generic;

namespace Api.Models;

public class DTOSchool
{
    public int Id { get; set; }
    public int AgencyId { get; set; }
    public string Name { get; set; }
    public DateTime? StartDate { get; set; }
    public string Address { get; set; }
    public int CityId { get; set; }
    public int RegionId { get; set; }
    public string ZipCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string PostalAddress { get; set; }
    public int? PostalCityId { get; set; }
    public int? PostalRegionId { get; set; }
    public string PostalZipCode { get; set; }
    public bool? SameAsPhysicalAddress { get; set; }

    // Información Administrativa
    public int OrganizationTypeId { get; set; }
    public int? CenterTypeId { get; set; }
    public bool? NonProfit { get; set; }
    public int? BaseYear { get; set; }
    public int? RenewalYear { get; set; }

    // Información Operacional
    public int EducationLevelId { get; set; }
    public int? OperatingDays { get; set; }
    public int? KitchenTypeId { get; set; }
    public int? GroupTypeId { get; set; }
    public int? DeliveryTypeId { get; set; }
    public int? SponsorTypeId { get; set; }
    public int? ApplicantTypeId { get; set; }
    public int? ResidentialTypeId { get; set; }
    public int? OperatingPolicyId { get; set; }
    public bool? HasWarehouse { get; set; }
    public bool? HasDiningRoom { get; set; }

    // Administrador/Representante Autorizado
    public string AdministratorAuthorizedName { get; set; }
    public string SitePhone { get; set; }
    public string Extension { get; set; }
    public string MobilePhone { get; set; }

    // Servicios y Horarios
    public bool? Breakfast { get; set; }
    public TimeSpan? BreakfastFrom { get; set; }
    public TimeSpan? BreakfastTo { get; set; }
    public bool? Lunch { get; set; }
    public TimeSpan? LunchFrom { get; set; }
    public TimeSpan? LunchTo { get; set; }
    public bool? Snack { get; set; }
    public TimeSpan? SnackFrom { get; set; }
    public TimeSpan? SnackTo { get; set; }

    // Dirección Física
    public DTOCity? City { get; set; }
    public DTORegion? Region { get; set; }
    public DTOCity? PostalCity { get; set; }
    public DTORegion? PostalRegion { get; set; }
    public DTOEducationLevel? EducationLevel { get; set; }
    public DTOOperatingPeriod? OperatingPeriod { get; set; }
    public DTOOrganizationType? OrganizationType { get; set; }
    public DTOKitchenType? KitchenType { get; set; }
    public DTOGroupType? GroupType { get; set; }
    public DTODeliveryType? DeliveryType { get; set; }
    public DTOSponsorType? SponsorType { get; set; }
    public DTOApplicantType? ApplicantType { get; set; }
    public DTOResidentialType? ResidentialType { get; set; }
    public DTOOperatingPolicy? OperatingPolicy { get; set; }
    public DTOCenterType? CenterType { get; set; }
    public DTOAgency? Agency { get; set; }
    public bool IsMainSchool { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
