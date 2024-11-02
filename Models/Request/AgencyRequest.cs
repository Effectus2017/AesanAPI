namespace Api.Models;

/// ------------------------------------------------------------------------------------------------
/// Modelo de registro de agencia
/// ------------------------------------------------------------------------------------------------

public class AgencyRequest
{
    public string Name { get; set; } = "";
    public int StatusId { get; set; } = 1;
    public int ProgramId { get; set; } = 0;

    // Datos de la Agencia
    public int SdrNumber { get; set; } = 0;
    public int UieNumber { get; set; } = 0;
    public int EinNumber { get; set; } = 0;

    // Datos de la Ciudad y Región
    public int CityId { get; set; } = 0;
    public int RegionId { get; set; } = 0;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;

    // Dirección y Teléfono
    public string Address { get; set; } = "";
    public int ZipCode { get; set; } = 0;
    public string PostalAddress { get; set; } = "";
    public string Phone { get; set; } = "";

    // Imágen - Logo
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? AdministrationTitle { get; set; }
    // Nuevos campos de elegibilidad
    public bool NonProfit { get; set; } = false;
    public bool FederalFundsDenied { get; set; } = false;
    public bool StateFundsDenied { get; set; } = false;
}
