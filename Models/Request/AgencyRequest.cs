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
    public int ZipCode { get; set; } = 0;
    public int CityId { get; set; } = 0;
    public int RegionId { get; set; } = 0;
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;

    // Dirección Postal
    public string PostalAddress { get; set; } = "";
    public int PostalCityId { get; set; } = 0;
    public int PostalRegionId { get; set; } = 0;
    public int PostalZipCode { get; set; } = 0;

    // Imágen - Logo
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = "";
    public string? AdministrationTitle { get; set; }
    // Nuevos campos de elegibilidad
    public bool NonProfit { get; set; } = false;
    public bool FederalFundsDenied { get; set; } = false;
    public bool StateFundsDenied { get; set; } = false;
    public bool OrganizedAthleticPrograms { get; set; } = false;
    public bool AtRiskService { get; set; } = false;
    // Campos de estado
    public bool IsActive { get; set; } = true;
    public bool IsListable { get; set; } = true;
    // Programas
    public List<int> Programs { get; set; } = [];
    // Codigo de Agencia
    public string? AgencyCode { get; set; }
}
