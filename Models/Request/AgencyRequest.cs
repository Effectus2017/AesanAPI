namespace Api.Models;

/// ------------------------------------------------------------------------------------------------
/// Modelo de registro de agencia
/// ------------------------------------------------------------------------------------------------

public class AgencyRequest
{
    public string Name { get; set; } = "";
    public int StatusId { get; set; } = 1;
    public int CityId { get; set; } = 0;
    public int RegionId { get; set; } = 0;
    public int ProgramId { get; set; } = 0;

    public int SdrNumber { get; set; } = 0;
    public int UieNumber { get; set; } = 0;
    public int EinNumber { get; set; } = 0;

    public string Address { get; set; } = "";
    public int PostalCode { get; set; } = 0;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
    public string Phone { get; set; } = "";
}
