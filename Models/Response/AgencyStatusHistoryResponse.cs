namespace Api.Models.Response;

/// <summary>
/// Una fila del listado paginado (primer result set del SP 100_GetAllAgencyStatusHistory).
/// El count viene en el segundo result set; mismo patrón que 109_GetAllUsersFromDb / 101_GetUserAssignedAgencies.
/// </summary>
public class AgencyStatusHistoryResponse
{
    public long Id { get; set; }
    public int AgencyId { get; set; }
    /// <summary>Nombre de la agencia. En el frontend se muestra como "Auspiciador".</summary>
    public string? AgencyName { get; set; }
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
    public string ChangedBy { get; set; } = "";
    public string? ChangedByName { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Justification { get; set; }
}
