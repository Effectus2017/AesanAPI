using System;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para información específica de Day Care Homes
/// </summary>
public class SchoolDayCareHomeResponse
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public bool? IsAuthorizedToOperate { get; set; }
    public bool? HasFamilyDepartmentLicense { get; set; }
    public int? NumberOfEnrolledChildren { get; set; }
    public int? NumberOfProviderChildren { get; set; }
    public int? NumberOfParticipantsWithBloodTies { get; set; }
    public int? NumberOfParticipantsWithoutBloodTies { get; set; }
    public bool? MinorsLiveWithProvider { get; set; }

    /// <summary>
    /// Información del tipo de relación
    /// </summary>
    public DTOOptionSelection? RelationshipType { get; set; }

    public bool? OffersServiceToImmigrantChildren { get; set; }

    /// <summary>
    /// Información del tipo de hogar
    /// </summary>
    public DTOOptionSelection? HomeType { get; set; }

    public string? AdministratorAuthorizedName { get; set; }
    public DateTime? AdministratorBirthDate { get; set; }
    public bool? OffersServiceToDifferentGroups { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
