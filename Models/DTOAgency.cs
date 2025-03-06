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
    public int? ZipCode { get; set; } = 0;

    // Dirección Postal
    public string PostalAddress { get; set; } = "";
    public int? PostalZipCode { get; set; } = 0;

    // Teléfono
    public string Phone { get; set; } = "";

    // Coordenadas
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;

    // Datos del Contacto
    public string Email { get; set; } = "";

    // Datos de Auditoría
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    // Imágen - Logo
    public string ImageURL { get; set; } = "";
    public string RejectionJustification { get; set; } = "";
    // Comentarios
    public string Comment { get; set; } = "";
    // Cita coordinada
    public bool? AppointmentCoordinated { get; set; }
    // Fecha de la cita
    public DateTime? AppointmentDate { get; set; } = DateTime.Now;

    // Relaciones
    public DTOCity City { get; set; } = new DTOCity();
    public DTORegion Region { get; set; } = new DTORegion();
    // Dirección Postal
    public DTOCity? PostalCity { get; set; } = new DTOCity();
    public DTORegion? PostalRegion { get; set; } = new DTORegion();
    // Estatus
    public DTOAgencyStatus Status { get; set; } = new DTOAgencyStatus();
    // Usuario
    public DTOUser? User { get; set; } = new DTOUser();
    // Usuario Monitor
    public DTOUser? Monitor { get; set; } = new DTOUser();
    // Programas
    public List<DTOProgram> Programs { get; set; } = [];

    // Código de la Agencia
    public string AgencyCode { get; set; } = "";
}
