namespace Api.Models;

public class DTOAgencyStatusHistory
{
    public long Id { get; set; }
    public int AgencyId { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; } = "";
    public string ChangedBy { get; set; } = "";
    public string ChangedByName { get; set; } = "";
    public DateTime ChangedAt { get; set; }
    public string? Justification { get; set; }
    public int? PreviousStatusId { get; set; }
    public string? PreviousStatusName { get; set; }
    /// <summary>Usado por el SP paginado; no se expone en la API.</summary>
    public int? TotalCount { get; set; }
}
