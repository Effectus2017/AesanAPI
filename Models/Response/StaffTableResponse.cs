namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para filas de la tabla/listado de staff (empleados, etc.).
/// Solo incluye las propiedades que se muestran en columnas en el frontend.
/// </summary>
public class StaffTableResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string FatherLastName { get; set; } = string.Empty;
    public string? MotherLastName { get; set; }
    public string? StaffClassificationName { get; set; }
    public string? DisplayPosition { get; set; }
    public bool IsActive { get; set; }
    public string? Comments { get; set; }
}
