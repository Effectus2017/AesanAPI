using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de reportes
/// </summary>
public interface IReportsRepository
{
    /// <summary>
    /// Obtiene la estructura jerárquica completa para el árbol de jerarquía de escuelas
    /// </summary>
    /// <param name="year">Año para filtrar la estructura</param>
    /// <param name="sponsorId">ID del auspiciador (opcional). Si es null, obtiene todos los auspiciadores</param>
    /// <returns>Estructura jerárquica con Auspiciador → Año → Escuelas → Sitios</returns>
    Task<HierarchyStructureResponse> GetHierarchyStructure(int year, int? sponsorId = null);
}

