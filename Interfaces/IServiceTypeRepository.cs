using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Repositorio para tipos de servicio por programa (AESAN-257).
/// </summary>
public interface IServiceTypeRepository
{
    /// <summary>
    /// Obtiene los tipos de servicio válidos para un programa, con IsStrongService y MinimumMinutesToNextService.
    /// </summary>
    /// <param name="programId">ID del programa.</param>
    /// <returns>Lista de ServiceTypeByProgramResponse.</returns>
    Task<IEnumerable<ServiceTypeByProgramResponse>> GetServiceTypesByProgram(int programId);
}
