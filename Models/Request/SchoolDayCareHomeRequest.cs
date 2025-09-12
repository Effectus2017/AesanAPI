using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para información específica de Day Care Homes
/// </summary>
public class SchoolDayCareHomeRequest
{
    public int? Id { get; set; }
    public int SchoolId { get; set; }
    public bool? IsAuthorizedToOperate { get; set; }
    public bool? HasFamilyDepartmentLicense { get; set; }
    public int? NumberOfEnrolledChildren { get; set; }
    public int? NumberOfProviderChildren { get; set; }
    public int? NumberOfParticipantsWithBloodTies { get; set; }
    public int? NumberOfParticipantsWithoutBloodTies { get; set; }
    public bool? MinorsLiveWithProvider { get; set; }

    /// <summary>
    /// ID del tipo de relación
    /// </summary>
    public int? RelationshipTypeId { get; set; }

    public bool? OffersServiceToImmigrantChildren { get; set; }

    /// <summary>
    /// ID del tipo de hogar
    /// </summary>
    public int? HomeTypeId { get; set; }

    public string? AdministratorAuthorizedName { get; set; }
    public DateTime? AdministratorBirthDate { get; set; }
    public bool? OffersServiceToDifferentGroups { get; set; }
}
