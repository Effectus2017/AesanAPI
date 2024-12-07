
namespace Api.Models;

public class DTOProgramInscription
{
    public int Id { get; set; }
    public DTOAgency? Agency { get; set; }
    public DTOProgram? Program { get; set; }
    public string ApplicationNumber { get; set; }
    public bool IsPublic { get; set; }
    public int TotalNumberSchools { get; set; }
    public bool HasBasicEducationCertification { get; set; }
    public bool IsAeaMenuCreated { get; set; }
    public string ExemptionRequirement { get; set; }
    public string ExemptionStatus { get; set; }
    public DTOFoodAuthority? ParticipatingAuthority { get; set; }
    public DTOOperatingPolicy? OperatingPolicy { get; set; }
    public bool HasCompletedCivilRightsQuestionnaire { get; set; }
    public bool NeedsInformationInOtherLanguages { get; set; }
    public string? InformationInOtherLanguages { get; set; }
    public bool NeedsInterpreter { get; set; }
    public string? InterpreterLanguages { get; set; }
    public bool NeedsAlternativeCommunication { get; set; }
    public DTOAlternativeCommunication? AlternativeCommunication { get; set; }
    public DTOOptionSelection? NeedsFederalRelayService { get; set; }
    public DTOOptionSelection? ShowEvidence { get; set; }
    public string? ShowEvidenceDescription { get; set; }
    public decimal? SnackPercentage { get; set; }
    public decimal? ReducedSnackPercentage { get; set; }
    public DTOFederalFundingCertification? FederalFundingCertification { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DTOSchool> Schools { get; set; }
    public List<DTOFederalFundingSource> FederalFundingSources { get; set; }
    public List<DTODocumentsRequired> RequiredDocuments { get; set; }
}
