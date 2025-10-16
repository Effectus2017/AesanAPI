using System;

namespace Api.Models.Request;

public class SiteDayCareHomeRequest
{
    public bool? IsAuthorizedToOperate { get; set; }
    public bool? HasFamilyDepartmentLicense { get; set; }
    public int? NumberOfEnrolledChildren { get; set; }
    public int? NumberOfProviderChildren { get; set; }
    public int? NumberOfParticipantsWithBloodTies { get; set; }
    public int? NumberOfParticipantsWithoutBloodTies { get; set; }
    public bool? MinorsLiveWithProvider { get; set; }
    public int? RelationshipTypeId { get; set; }
    public bool? OffersServiceToImmigrantChildren { get; set; }
    public int? HomeTypeId { get; set; }
    public DateTime? AdministratorBirthDate { get; set; }
    public bool? OffersServiceToDifferentGroups { get; set; }
}
