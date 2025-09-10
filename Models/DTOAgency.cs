namespace Api.Models;

public class DTOAgency
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public string Name { get; set; } = "";
    // Datos de la Agencia
    public int SdrNumber { get; set; } = 0;
    public int UieNumber { get; set; } = 0;
    public int EinNumber { get; set; } = 0;

    // Datos de la Ciudad y Región
    public string Address { get; set; } = "";
    public string ZipCode { get; set; } = "";

    // Dirección Postal
    public string PostalAddress { get; set; } = "";
    public string PostalZipCode { get; set; } = "";

    // Teléfono
    public string Phone { get; set; } = "";

    // Coordenadas
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Datos del Contacto
    public string Email { get; set; } = "";

    // Datos de Auditoría
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    // Imágen - Logo
    public string ImageURL { get; set; } = "";

    // Relaciones
    public DTOCity? City { get; set; } = new DTOCity();
    public DTORegion? Region { get; set; } = new DTORegion();
    // Dirección Postal
    public DTOCity? PostalCity { get; set; } = new DTOCity();
    public DTORegion? PostalRegion { get; set; } = new DTORegion();
    // Estatus
    public DTOAgencyStatus? Status { get; set; } = new DTOAgencyStatus();
    // Usuario
    public DTOStaff? User { get; set; } = new DTOStaff();
    // Usuario Monitor
    public DTOStaff? Monitor { get; set; } = new DTOStaff();
    // Programas
    public List<DTOProgram> Programs { get; set; } = [];
    // Código de la Agencia
    public string AgencyCode { get; set; } = "";

    // Datos de inscripción de la agencia
    public DTOAgencyInscription? Inscription { get; set; }
}
