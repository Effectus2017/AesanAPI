namespace Api.Models.Request;

/// <summary>Body opcional para aprobar solicitud de extensión (nueva fecha tope).</summary>
public class ApproveExtensionRequest
{
    public DateTime? NewValidTo { get; set; }
}
