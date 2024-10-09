namespace Api.Models;

public class QueryParams
{
    public int Take { get; set; } = 1; // Número de página por defecto
    public int Skip { get; set; } = 10; // Tamaño de página por defecto
    public string Name { get; set; } = ""; // Nombre del usuario
}
