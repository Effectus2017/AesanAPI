using System;

namespace Api.Models.Response;

public class SiteDayCareHomeResponse
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public bool? IsAuthorizedToOperate { get; set; }
    public bool? HasFamilyDepartmentLicense { get; set; }
    public int? NumberOfEnrolledChildren { get; set; }
    public int? NumberOfProviderChildren { get; set; }
    public int? NumberOfParticipantsWithBloodTies { get; set; }
    public int? NumberOfParticipantsWithoutBloodTies { get; set; }
    public bool? MinorsLiveWithProvider { get; set; }
    public DTOOptionSelection? RelationshipType { get; set; }
    public bool? OffersServiceToImmigrantChildren { get; set; }
    public DTOOptionSelection? HomeType { get; set; }
    public string? AdministratorAuthorizedName { get; set; }
    public DateTime? AdministratorBirthDate { get; set; }
    public bool? OffersServiceToDifferentGroups { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
