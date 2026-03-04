using Api.Models;
using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de School
/// </summary>
public interface ISchoolRepository
{
    /// <summary>
    /// Obtiene una escuela por su ID
    /// </summary>
    Task<SchoolResponse> GetSchoolById(int id);

    /// <summary>
    /// Obtiene todas las escuelas con paginación y filtros
    /// </summary>
    Task<dynamic> GetAllSchools(int take, int skip, string? name, int? agencyId, bool alls);

    /// <summary>
    /// Inserta una nueva escuela
    /// </summary>
    Task<bool> InsertSchool(SchoolRequest request);

    /// <summary>
    /// Actualiza una escuela existente
    /// </summary>
    Task<bool> UpdateSchool(SchoolRequest request);

    /// <summary>
    /// Elimina una escuela (soft delete)
    /// </summary>
    Task<bool> DeleteSchool(int id);

    /// <summary>
    /// Obtiene todas las escuelas de una agencia específica con paginación y filtros
    /// </summary>
    Task<dynamic> GetSchoolsByAgencyId(int agencyId, int take, int skip, string? name, bool? isActive, string? schoolCode);
}
