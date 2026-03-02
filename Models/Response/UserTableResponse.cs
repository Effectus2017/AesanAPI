namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para filas de la tabla de usuarios (solo columnas visibles).
/// </summary>
public class UserTableResponse
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? FatherLastName { get; set; }
    public bool IsActive { get; set; }
    public string? RoleNames { get; set; }
}
