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
    // Registro de Educación Básica
    public int BasicEducationRegistry { get; set; } = 0;
    // Programas
    public List<int> Programs { get; set; } = [];
    // Codigo de Agencia
    public string? AgencyCode { get; set; }
    // Monitor
    public string? MonitorId { get; set; }
    // Usuario que asigna
    public string? AssignedBy { get; set; }
    // Service Time
    public DateTime ServiceTime { get; set; } = DateTime.MinValue;
    // Tax Exemption
    public int TaxExemptionStatus { get; set; } = 0;
    public int TaxExemptionType { get; set; } = 0;
    // Nombres
    public string FirstName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string MotherLastName { get; set; } = "";
}
