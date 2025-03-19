namespace Api.Models;

/// ------------------------------------------------------------------------------------------------
/// Modelo de solicitud para actualizar el programa de una agencia
/// ------------------------------------------------------------------------------------------------
public class UpdateAgencyProgramRequest
{
    public int AgencyId { get; set; } = 0;
    public int ProgramId { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string UserId { get; set; } = "";
    public string RejectionJustification { get; set; } = "";
    public bool AppointmentCoordinated { get; set; } = false;
    public DateTime? AppointmentDate { get; set; }
}
