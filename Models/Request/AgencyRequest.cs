namespace Api.Models;

/// ------------------------------------------------------------------------------------------------
/// Modelo de registro de agencia
/// ------------------------------------------------------------------------------------------------

public class AgencyRequest
{
    public string Name { get; set; } = "";
    public int StatusId { get; set; } = 1;

    // Datos de la Agencia
    public int SdrNumber { get; set; } = 0;
    public int UieNumber { get; set; } = 0;
    public int EinNumber { get; set; } = 0;

    // Dirección Física
    public string Address { get; set; } = "";
    public string ZipCode { get; set; } = "";
    public int CityId { get; set; } = 0;
    public int RegionId { get; set; } = 0;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;

    // Dirección Postal
    public string PostalAddress { get; set; } = "";
    public int PostalCityId { get; set; } = 0;
    public int PostalRegionId { get; set; } = 0;
    public string PostalZipCode { get; set; } = "";

    // Imágen - Logo
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = "";
    public string? AdministrationTitle { get; set; }

    // Campos de estado
    public bool IsActive { get; set; } = true;
    public bool IsListable { get; set; } = true;
    // Registro de Educación Básica

    // Programas
    public List<int> Programs { get; set; } = [];
    // Codigo de Agencia
    public string? AgencyCode { get; set; }
    // Monitor
    public string? MonitorId { get; set; }
    // Usuario que asigna
    public string? AssignedBy { get; set; }

    // ------------------------------------------------------------
    // Para Agencia Inscripción (REGISTRO)
    // ------------------------------------------------------------

    // Service Time
    public DateTime ServiceTime { get; set; } = DateTime.MinValue;
    // ¿Es una organización sin fines de lucro?
    // Is it a non-profit organization?
    // Si (1) y No (2)
    public bool NonProfit { get; set; } = false;
    // ¿Posee Certificación de Registro de Educación Básica?
    // Do you have Basic Education Registry Certification?
    // En Proceso (3), Otorgado (4), Denegado (5)
    public int BasicEducationRegistryId { get; set; } = 0;
    // ¿Ha sido denegado o descalificado de fondos estatales en los últimos siete años?
    // Have you been denied or disqualified from state funds in the last seven years?
    // Si (1) y No (2)
    public bool StateFundsDenied { get; set; } = false;
    // ¿Ha sido denegado o descalificado de fondos federales en los últimos siete años?
    // Have you been denied or disqualified from federal funds in the last seven years?
    // Si (1) y No (2)
    public bool FederalFundsDenied { get; set; } = false;
    // ¿El Auspiciador ofrece programas atléticos organizados que participan en deportes competitivos interescolares o a nivel comunitario?
    // Does the Sponsor offer any organized athletic programs engaged in interscholastic or community level competitive sports?
    // Si (1) y No (2)
    public bool OrganizedAthleticPrograms { get; set; } = false;
    // ¿Está interesado en participar en el servicio de merienda y cena en riesgo?
    // Is the Sponsor interested in participating in the at-risk snack and dinner service?
    // Si (1) y No (2)
    public bool AtRiskService { get; set; } = false;
    // ¿En qué estatus se encuentra su Exención Contributiva?
    // In what status is your Tax Exemption?
    // En Proceso (3), Otorgado (4), Denegado (5)
    public int TaxExemptionStatusId { get; set; } = 0;
    // ¿Qué tipo de Exención Contributiva tiene?
    // What type of Tax Exemption does it have?
    // Estatal (11), Federal (12)
    public int TaxExemptionTypeId { get; set; } = 0;
    // Tipo de Entidad
    // Type of Entity
    // Gobierno (13), Privado (14)
    public int TypeOfEntityId { get; set; } = 0;
    // Tipo de Solicitante
    // Type of Applicant
    // Laico (15), Base de fe (16)
    public int TypeOfApplicantId { get; set; } = 0;
    // ¿De poseer un contrato Público Alianza especifique su modalidad?
    // If you have a Public Alliance contract, please specify the type of contract
    // Socio-Económico (17), Híbrido (18)
    public int PublicAllianceContractId { get; set; } = 0;
    // ¿Su Institución es un Programa Nacional de Juventud?
    // Is your institution a National Youth Program?
    // Si (1) y No (2)
    public bool NationalYouthProgram { get; set; } = false;

    // Nombres
    public string FirstName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string MotherLastName { get; set; } = "";

}
