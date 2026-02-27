using Api.Models;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para una agencia (contrato de API).
/// </summary>
public class AgencyResponse
{
    public int Id { get; set; }
    public int StatusId { get; set; }
    public string Name { get; set; } = "";

    public long SdrNumber { get; set; }
    public long UieNumber { get; set; }
    public int EinNumber { get; set; }

    public string Address { get; set; } = "";
    public string ZipCode { get; set; } = "";
    public string PostalAddress { get; set; } = "";
    public string PostalZipCode { get; set; } = "";
    public string Phone { get; set; } = "";

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Email { get; set; } = "";

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string ImageURL { get; set; } = "";
    public string AgencyCode { get; set; } = "";
    public bool IsRecurrent { get; set; }

    public DTOCity? City { get; set; }
    public DTORegion? Region { get; set; }
    public DTOCity? PostalCity { get; set; }
    public DTORegion? PostalRegion { get; set; }
    public AgencyStatusResponse? Status { get; set; }
    public DTOStaff? User { get; set; }
    public List<DTOStaff> AssignedUsers { get; set; } = [];
    public List<DTOProgram> Programs { get; set; } = [];
    public AgencyInscriptionResponse? Inscription { get; set; }
}
