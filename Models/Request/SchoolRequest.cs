namespace Api.Models;
using System;
using System.Collections.Generic;

public class SchoolRequest
{
    public int? Id { get; set; }
    public string Name { get; set; }

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
    public int? CenterId { get; set; }
    public bool? NonProfit { get; set; }
    public DateTime? StartDate { get; set; }
    public int? BaseYear { get; set; }
    public int? RenewalYear { get; set; }
    public int EducationLevelId { get; set; }
    public int? OperatingDays { get; set; }

    // Información Operacional
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

    // Escuela Principal
    public bool? IsMainSchool { get; set; }
}