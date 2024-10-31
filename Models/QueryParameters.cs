namespace Api.Models;

public class QueryParameters
{
    public int Take { get; set; } = 1; // Número de página por defecto
    public int Skip { get; set; } = 10; // Tamaño de página por defecto
    public string Name { get; set; } = ""; // Nombre del usuario
    public bool Alls { get; set; } = false; // Mostrar todos los registros
    public string UserId { get; set; } = ""; // ID del usuario
    public int? RegionId { get; set; } // ID de la región
    public int? CityId { get; set; } // ID de la ciudad
    public int? ProgramId { get; set; } // ID del programa
    public int? StatusId { get; set; } // ID del estado
    public int? AgencyId { get; set; } // ID de la agencia
}
