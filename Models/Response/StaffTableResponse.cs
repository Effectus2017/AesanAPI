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
    public string? PositionName { get; set; }
    // Se muetra en el modal de agregar relación
    public string? StaffTypeName { get; set; }
    // Se muetra en el modal de agregar relación
    public string? StaffTypeNameEn { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Email { get; set; }
    // Se muestra en la tabla de miembros de la junta
    public bool HasRelationships { get; set; }
    public bool IsActive { get; set; }
    public string? Comments { get; set; }
}
