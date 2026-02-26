namespace Api.Models;

/// <summary>
/// DTO que refleja la tabla AgencyStatusHistory (solo columnas de la tabla).
/// </summary>
public class DTOAgencyStatusHistory
{
    public long Id { get; set; }
    public int AgencyId { get; set; }
    public int StatusId { get; set; }
    /// <summary>UserId (AspNetUsers.Id) de quien realizó el cambio.</summary>
    public string ChangedBy { get; set; } = "";
    public DateTime ChangedAt { get; set; }
    public string? Justification { get; set; }
}
