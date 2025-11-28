using System.ComponentModel.DataAnnotations;

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

    [MaxLength(12, ErrorMessage = "El número UIE debe tener máximo 12 caracteres")]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "El número UIE solo puede contener letras y números")]
    public string UieNumber { get; set; } = "";

    [Range(0, 999999999, ErrorMessage = "El número EIN debe tener máximo 9 dígitos")]
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

    // Imagen - Logo
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
    // Código de Agencia
    public string? AgencyCode { get; set; }
    // Monitor
    public int? MonitorId { get; set; }
    // Usuario que asigna
    public string? AssignedBy { get; set; }

    // ------------------------------------------------------------
    // Para Agencia Inscripción (REGISTRO)
    // ------------------------------------------------------------
    // ¿Es una organización sin fines de lucro?
    // Is it a non-profit organization?
    // Si (1) y No (2)
    public bool NonProfit { get; set; } = false;
    // ¿Posee Certificación de Registro de Educación Básica?
    // Do you have Basic Education Registry Certification?
    // Si (1) y No (2)
    public bool BasicEducationRegistry { get; set; } = false;
    // ¿Está interesado en participar de horario extendido? (Solo para PACNA)
    // Are you interested in participating in extended hours? (Only for PACNA)
    // Si (1) y No (2)
    public bool ExtendedHours { get; set; } = false;
    // ¿Desde cuándo su Entidad ofrece servicios? (Solo para PACNA)
    public DateTime? ServicesOfferedSince { get; set; }
    // ¿Ha sido denegado o descalificado de fondos estatales en los últimos siete años?
    // Have you been denied or disqualified from state funds in the last seven years?
    // Si (1) y No (2)
    public bool StateFundsDenied { get; set; } = false;
    // ¿Razón por la cual fue descalificado o denegado de fondos estatales?
    // Reason why the sponsor was disqualified or denied state funds?
    // Se activa cuando StateFundsDenied = true
    public string? StateFundsDeniedReason { get; set; }
    // ¿Ha sido denegado o descalificado de fondos federales en los últimos siete años?
    // Have you been denied or disqualified from federal funds in the last seven years?
    // Si (1) y No (2)
    public bool FederalFundsDenied { get; set; } = false;
    // ¿Razón por la cual fue descalificado o denegado de fondos federales?
    // Reason why the sponsor was disqualified or denied federal funds?
    // Se activa cuando FederalFundsDenied = true
    public string? FederalFundsDeniedReason { get; set; }
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
    // De poseer un contrato Público Alianza, especifique su modalidad
    // If you have a Public Alliance contract, please specify the type of contract
    // Socio-Económico (17), Híbrido (18)
    public int? PublicAllianceContractId { get; set; } = null;
    // ¿Su Institución es un Programa Nacional de Juventud?
    // Is your institution a National Youth Program?
    // Si (1) y No (2)
    public bool NationalYouthProgram { get; set; } = false;
    // ¿Es usted una Agencia Auspiciadora de Hogares? (Solo para programa PACNA)
    // Are you a Day Care Homes? (Only for PACNA program)
    // No (ID), Sí (ID), Ambos (ID) - Ahora usa OptionSelection
    public int? IsDayCareHomeId { get; set; } = null;
    // ¿Su Entidad participa actualmente en alguno de los siguientes programas? (Solo para PSAV)
    // Does your Entity currently participate in any of the following programs? (Only for PSAV)
    // Early Head Start, Head Start, N/A
    public int? ParticipatesInHeadStartProgramId { get; set; } = null;

    // ¿Es la agencia recurrente?
    // Is the agency recurrent?
    public bool IsRecurrent { get; set; } = false;

    // Nombres
    public string FirstName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string MotherLastName { get; set; } = "";

}
