namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para datos de inscripción de agencia
/// Incluye todos los campos específicos del proceso de inscripción de una agencia
/// </summary>
public class AgencyInscriptionResponse
{
    public int Id { get; set; }

    /// <summary>
    /// ID de la agencia asociada
    /// </summary>
    public int AgencyId { get; set; }

    /// <summary>
    /// Si es una organización sin fines de lucro
    /// </summary>
    public bool? NonProfit { get; set; }

    /// <summary>
    /// Si la agencia no acepta fondos federales
    /// </summary>
    public bool? FederalFundsDenied { get; set; }

    /// <summary>
    /// Razón por la cual fue descalificado o denegado de fondos federales
    /// Se activa cuando FederalFundsDenied = true
    /// </summary>
    public string? FederalFundsDeniedReason { get; set; }

    /// <summary>
    /// Si la agencia no acepta fondos estatales
    /// </summary>
    public bool? StateFundsDenied { get; set; }

    /// <summary>
    /// Razón por la cual fue descalificado o denegado de fondos estatales
    /// Se activa cuando StateFundsDenied = true
    /// </summary>
    public string? StateFundsDeniedReason { get; set; }

    /// <summary>
    /// Si la agencia tiene registro de educación básica
    /// Si (1) y No (2)
    /// </summary>
    public bool? BasicEducationRegistry { get; set; }

    /// <summary>
    /// Si está interesado en participar de horario extendido (Solo para programa PACNA)
    /// Si (1) y No (2)
    /// </summary>
    public bool? ExtendedHours { get; set; }

    /// <summary>
    /// ¿Desde cuándo su Entidad ofrece servicios? (Solo para programa PACNA)
    /// </summary>
    public DateTime? ServicesOfferedSince { get; set; }

    /// <summary>
    /// Justificación de rechazo de la agencia
    /// </summary>
    public string? RejectionJustification { get; set; }

    /// <summary>
    /// Comentarios adicionales para la agencia
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// Si la agencia tiene cita programada
    /// </summary>
    public bool? AppointmentCoordinated { get; set; }

    /// <summary>
    /// Fecha de la cita programada
    /// </summary>
    public DateTime? AppointmentDate { get; set; }

    /// <summary>
    /// Estatus de la Exención Contributiva
    /// En Proceso (3), Otorgado (4), Denegado (5)
    /// </summary>
    public int? TaxExemptionStatusId { get; set; }

    /// <summary>
    /// Tipo de Exención Contributiva
    /// Estatal (11), Federal (12)
    /// </summary>
    public int? TaxExemptionTypeId { get; set; }

    /// <summary>
    /// Tipo de Entidad
    /// Privado (14), Gobierno (15)
    /// </summary>
    public int? TypeOfEntityId { get; set; }

    /// <summary>
    /// Tipo de Solicitante
    /// Laico (16), Base de fe (17)
    /// </summary>
    public int? TypeOfApplicantId { get; set; }

    /// <summary>
    /// Modalidad de contrato Público Alianza
    /// Socio-Económico (17), Híbrido (18)
    /// </summary>
    public int? PublicAllianceContractId { get; set; }

    /// <summary>
    /// Si es un Programa Nacional de Juventud
    /// </summary>
    public bool? NationalYouthProgram { get; set; }

    /// <summary>
    /// Si es una Agencia Auspiciadora de Hogares (Solo para programa PACNA)
    /// Ahora usa OptionSelection con opciones: No, Sí, Ambos
    /// </summary>
    public int? IsDayCareHomeId { get; set; }

    /// <summary>
    /// Fecha límite para completar la inscripción de los Sitios
    /// </summary>
    public DateTime? DeadlineToCompleteRegistration { get; set; }

    /// <summary>
    /// Fecha de registro de la inscripción completada
    /// </summary>
    public DateTime? CompletedRegistrationDate { get; set; }

    // Relaciones con OptionSelection para los campos de selección
    /// <summary>
    /// Estatus de la exención contributiva (relación)
    /// </summary>
    public DTOOptionSelection? TaxExemptionStatus { get; set; }

    /// <summary>
    /// Tipo de exención contributiva (relación)
    /// </summary>
    public DTOOptionSelection? TaxExemptionType { get; set; }

    /// <summary>
    /// Tipo de entidad (relación)
    /// </summary>
    public DTOOptionSelection? TypeOfEntity { get; set; }

    /// <summary>
    /// Tipo de solicitante (relación)
    /// </summary>
    public DTOOptionSelection? TypeOfApplicant { get; set; }

    /// <summary>
    /// Modalidad de contrato público alianza (relación)
    /// </summary>
    public DTOOptionSelection? PublicAllianceContract { get; set; }

    /// <summary>
    /// Si es una Agencia Auspiciadora de Hogares (relación)
    /// </summary>
    public DTOOptionSelection? IsDayCareHome { get; set; }

    /// <summary>
    /// ¿Su Entidad participa actualmente en alguno de los siguientes programas? (Solo para PSAV)
    /// Does your Entity currently participate in any of the following programs? (Only for PSAV)
    /// Early Head Start, Head Start, N/A
    /// </summary>
    public int? ParticipatesInHeadStartProgramId { get; set; }

    /// <summary>
    /// Participa en programa Head Start (relación)
    /// </summary>
    public DTOOptionSelection? ParticipatesInHeadStartProgram { get; set; }

    /// <summary>
    /// ¿Cuántas reuniones se realizan durante el año? (Solo para PACNA)
    /// How many meetings are held during the year? (Only for PACNA)
    /// </summary>
    public int? BoardMeetingsPerYear { get; set; }

    /// <summary>
    /// ¿La Junta de Directores se reúne regularmente? (Solo para PACNA)
    /// Does the Board of Directors meet regularly? (Only for PACNA)
    /// </summary>
    public bool? BoardMeetsRegularly { get; set; }

    /// <summary>
    /// Funciones de autoridad de la Junta de Directores (relación completa)
    /// </summary>
    public List<DTOOptionSelection>? BoardExecutiveAuthority { get; set; }
}

