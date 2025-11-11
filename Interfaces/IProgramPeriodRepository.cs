using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de ProgramPeriod
/// </summary>
public interface IProgramPeriodRepository
{
    /// <summary>
    /// Obtiene todos los períodos de un programa
    /// </summary>
    /// <param name="programId">ID del programa</param>
    /// <returns>Lista de períodos del programa</returns>
    Task<IEnumerable<ProgramPeriodResponse>> GetProgramPeriodsByProgramId(int programId);

    /// <summary>
    /// Obtiene un período específico de un programa por año
    /// </summary>
    /// <param name="programId">ID del programa</param>
    /// <param name="year">Año del período</param>
    /// <returns>Período encontrado o null</returns>
    Task<ProgramPeriodResponse?> GetProgramPeriodByProgramIdAndYear(int programId, int year);

    /// <summary>
    /// Inserta un nuevo período de programa
    /// </summary>
    /// <param name="request">Datos del período a insertar</param>
    /// <returns>ID del período insertado</returns>
    Task<int> InsertProgramPeriod(ProgramPeriodRequest request);

    /// <summary>
    /// Actualiza un período de programa existente
    /// </summary>
    /// <param name="request">Datos del período a actualizar</param>
    /// <returns>True si la actualización fue exitosa</returns>
    Task<bool> UpdateProgramPeriod(ProgramPeriodRequest request);
}

