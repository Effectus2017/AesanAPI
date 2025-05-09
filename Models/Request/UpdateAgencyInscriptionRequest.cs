namespace Api.Models;

/// ------------------------------------------------------------------------------------------------
/// Modelo de solicitud para actualizar la inscripción de una agencia
/// ------------------------------------------------------------------------------------------------
public class UpdateAgencyInscriptionRequest
{
    public int AgencyId { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string Comments { get; set; } = "";
    public bool AppointmentCoordinated { get; set; } = false;
    public DateTime? AppointmentDate { get; set; }
    public string? RejectionJustification { get; set; }
}

/// ------------------------------------------------------------------------------------------------