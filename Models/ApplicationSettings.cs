namespace Api.Models;

public class ApplicationSettings
{
    public string Secret { get; set; } = "";
    public string EmailFrom { get; set; } = "";
    public string EmailContacto { get; set; } = "";
    public string ConfirmationMailMessage { get; set; } = "";
    public string ConfirmationMailMessageSendGrid { get; set; } = "";
    public string VerificationMailMessage { get; set; } = "";
    public string VerificationMailMessageSendGrid { get; set; } = "";
    public string CompleteMailMessage { get; set; } = "";
    public string NewPasswordMailMessage { get; set; } = "";
    public string ResetPasswordMailMessage { get; set; } = "";

    public string LocalURL { get; set; } = "";
    public string StagingURL { get; set; } = "";
    public string ProduccionURL { get; set; } = "";

    public string LocalWebURL { get; set; } = "";
    public string StagingWebURL { get; set; } = "";
    public string ProduccionWebURL { get; set; } = "";

    // Cache settings
    public CacheSettings Cache { get; set; } = new CacheSettings();
    // Gmail settings
    public GmailSettings Gmail { get; set; } = new GmailSettings();
}

public class CacheSettings
{
    public bool Enabled { get; set; } = true;
    public int DefaultExpirationMinutes { get; set; } = 10;
    public int AgencyExpirationSeconds { get; set; } = 30;
    public CacheKeys Keys { get; set; } = new();
}

public class CacheKeys
{
    public string Cities { get; set; } = "Cities_{0}_{1}_{2}_{3}";
    public string Regions { get; set; } = "Regions_{0}_{1}_{2}_{3}";
    public string City { get; set; } = "City_{0}";
    public string Region { get; set; } = "Region_{0}";
    public string RegionsByCity { get; set; } = "RegionsByCity_{0}";
    public string CitiesByRegion { get; set; } = "CitiesByRegion_{0}";

    // Claves para Programs
    public string Programs { get; set; } = "Programs_{0}_{1}_{2}_{3}";
    public string Program { get; set; } = "Program_{0}";
    public string ProgramInscriptions { get; set; } = "ProgramInscriptions_{0}_{1}_{2}_{3}";
    public string OptionSelections { get; set; } = "OptionSelections";

    // Claves para Agency
    public string Agencies { get; set; } = "Agencies_{0}_{1}_{2}_{3}";
    public string Agency { get; set; } = "Agency_{0}";
    public string AgencyStatuses { get; set; } = "AgencyStatuses_{0}_{1}_{2}_{3}";
    public string AgencyStatus { get; set; } = "AgencyStatus_{0}";
    public string AgencyUsers { get; set; } = "AgencyUsers_{0}_{1}_{2}";

    // Claves para School
    public string Schools { get; set; } = "Schools_{0}_{1}_{2}_{3}";
    public string School { get; set; } = "School_{0}";

    // Claves para OrganizationType
    public string OrganizationTypes { get; set; } = "OrganizationTypes_{0}_{1}_{2}_{3}";
    public string OrganizationType { get; set; } = "OrganizationType_{0}";

    // Claves para OperatingPolicy
    public string OperatingPolicies { get; set; } = "OperatingPolicies_{0}_{1}_{2}_{3}";
    public string OperatingPolicy { get; set; } = "OperatingPolicy_{0}";

    // Claves para AlternativeCommunication
    public string AlternativeCommunications { get; set; } = "AlternativeCommunications_{0}_{1}_{2}_{3}";
    public string AlternativeCommunication { get; set; } = "AlternativeCommunication_{0}";

    // Claves para EducationLevel
    public string EducationLevels { get; set; } = "EducationLevels_{0}_{1}_{2}_{3}";
    public string EducationLevel { get; set; } = "EducationLevel_{0}";

    // Claves para Facility
    public string Facilities { get; set; } = "Facilities_{0}_{1}_{2}_{3}";
    public string Facility { get; set; } = "Facility_{0}";

    // Claves para FederalFundingCertification
    public string FederalFundingCertifications { get; set; } = "FederalFundingCertifications_{0}_{1}_{2}_{3}";
    public string FederalFundingCertification { get; set; } = "FederalFundingCertification_{0}";

    // Claves para FoodAuthority
    public string FoodAuthorities { get; set; } = "FoodAuthorities_{0}_{1}_{2}_{3}";
    public string FoodAuthority { get; set; } = "FoodAuthority_{0}";

    // Claves para MealType
    public string MealTypes { get; set; } = "MealTypes_{0}_{1}_{2}_{3}";
    public string MealType { get; set; } = "MealType_{0}";

    // Claves para OperatingPeriod
    public string OperatingPeriods { get; set; } = "OperatingPeriods_{0}_{1}_{2}_{3}";
    public string OperatingPeriod { get; set; } = "OperatingPeriod_{0}";
}

public class GmailSettings
{
    public string EmailFrom { get; set; } = "";
    public string SmtpServer { get; set; } = "";
    public int SmtpServerPort { get; set; } = 0;
    public string SmtpUser { get; set; } = "";
    public string SmtpPass { get; set; } = "";
    public string EmailToDev { get; set; } = "";
}
