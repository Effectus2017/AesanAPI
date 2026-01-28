namespace Api.Models.Response;

/// <summary>
/// Respuesta de tipo de servicio por programa (AESAN-257).
/// Incluye IsStrongService y MinimumMinutesToNextService desde ServiceTypeProgram.
/// </summary>
public class ServiceTypeByProgramResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEN { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsStrongService { get; set; }
    public int? MinimumMinutesToNextService { get; set; }
    public bool IsActive { get; set; }
}
