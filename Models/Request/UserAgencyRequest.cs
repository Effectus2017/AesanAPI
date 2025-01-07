namespace Api.Models;

/// ------------------------------------------------------------------------------------------------
/// Modelo de registro de usuario
/// ------------------------------------------------------------------------------------------------

public class UserAgencyRequest
{
    public AgencyRequest Agency { get; set; } = new AgencyRequest();
    public User User { get; set; } = new User();
}
