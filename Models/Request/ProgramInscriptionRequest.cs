namespace Api.Models;

public class ProgramInscriptionRequest
{
    public int AgencyId { get; set; }
    public int ProgramId { get; set; }
    public string ApplicationNumber { get; set; }
    public bool IsPublic { get; set; }
    public int TotalNumberSchools { get; set; }
    public bool HasBasicEducationCertification { get; set; }
    public bool IsAeaMenuCreated { get; set; }
    public string ExemptionRequirement { get; set; }
    public string ExemptionStatus { get; set; }
    public int ParticipatingAuthorityId { get; set; }
    public int OperatingPolicyId { get; set; }
    public bool HasCompletedCivilRightsQuestionnaire { get; set; }
    public bool NeedsInformationInOtherLanguages { get; set; }
    public string? InformationInOtherLanguages { get; set; }
    public bool NeedsInterpreter { get; set; }
    public string? InterpreterLanguages { get; set; }
    public bool NeedsAlternativeCommunication { get; set; }
    public int? AlternativeCommunicationId { get; set; }
    public int NeedsFederalRelayServiceId { get; set; }
    public int ShowEvidenceId { get; set; }
    public string? ShowEvidenceDescription { get; set; }
    public decimal? SnackPercentage { get; set; }
    public decimal? ReducedSnackPercentage { get; set; }
    public int? FederalFundingCertificationId { get; set; }
    public List<SchoolRequest>? Schools { get; set; }
    public List<FederalFundingSourceRequest>? FederalFundingSources { get; set; }
    public List<int>? RequiredDocumentIds { get; set; }
}