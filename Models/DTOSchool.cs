using Api.Models;
using System;
using System.Collections.Generic;

public class DTOSchool
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? StartDate { get; set; }
    public string Address { get; set; }
    public string PostalAddress { get; set; }
    public string ZipCode { get; set; }
    public int CityId { get; set; }
    public int RegionId { get; set; }
    public string AreaCode { get; set; }
    public string AdminFullName { get; set; }
    public string Phone { get; set; }
    public string PhoneExtension { get; set; }
    public string Mobile { get; set; }
    public int? BaseYear { get; set; }
    public int? NextRenewalYear { get; set; }
    public int OrganizationTypeId { get; set; }
    public int EducationLevelId { get; set; }
    public int OperatingPeriodId { get; set; }
    public int? KitchenTypeId { get; set; }
    public int? GroupTypeId { get; set; }
    public int? DeliveryTypeId { get; set; }
    public int? SponsorTypeId { get; set; }
    public int? ApplicantTypeId { get; set; }
    public int? OperatingPolicyId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public DTOCity City { get; set; }
    public DTORegion Region { get; set; }
    public DTOEducationLevel EducationLevel { get; set; }
    public DTOOperatingPeriod OperatingPeriod { get; set; }
    public DTOOrganizationType OrganizationType { get; set; }
    public List<DTOFacility> Facilities { get; set; }
    public List<DTOMealType> MealTypes { get; set; }
    public List<DTOSchoolFacility> SchoolFacilities { get; set; }
    public List<DTOSatelliteSchool> Satellites { get; set; }
}