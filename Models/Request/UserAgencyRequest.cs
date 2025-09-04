namespace Api.Models;
using Api.Models.Request;

/// ------------------------------------------------------------------------------------------------
/// Modelo de registro de usuario
/// ------------------------------------------------------------------------------------------------

public class UserAgencyRequest
{
    public AgencyRequest Agency { get; set; } = new AgencyRequest();
    public StaffRequest Staff { get; set; } = new StaffRequest();  // Cambiado de User a StaffRequest
}
