namespace Api.Models;

/// <summary>
/// DTO para transferir datos de inscripción de agencia
/// Incluye todos los campos específicos del proceso de inscripción de una agencia
/// </summary>
public class DTOAgencyInscription
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
    /// Si la agencia no acepta fondos estatales
    /// </summary>
    public bool? StateFundsDenied { get; set; }

    /// <summary>
    /// Si la agencia tiene programas de atletismo organizados
    /// </summary>
    public bool? OrganizedAthleticPrograms { get; set; }

    /// <summary>
    /// Si la agencia ofrece servicios a personas en riesgo
    /// </summary>
    public bool? AtRiskService { get; set; }

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
    /// Si la agencia tiene servicio de tiempo
    /// </summary>
    public DateTime? ServiceTime { get; set; }

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
    /// </summary>
    public bool? IsDayCareHome { get; set; }

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
}
