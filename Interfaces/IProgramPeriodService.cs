using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el servicio de ProgramPeriod
/// </summary>
public interface IProgramPeriodService
{
    /// <summary>
    /// Obtiene todos los períodos de un programa
    /// </summary>
    Task<IEnumerable<ProgramPeriodResponse>> GetProgramPeriodsByProgramId(int programId);

    /// <summary>
    /// Obtiene un período específico de un programa por año
    /// </summary>
    Task<ProgramPeriodResponse?> GetProgramPeriodByProgramIdAndYear(int programId, int year);

    /// <summary>
    /// Inserta un nuevo período de programa
    /// </summary>
    Task<ProgramPeriodResponse> InsertProgramPeriod(ProgramPeriodRequest request);

    /// <summary>
    /// Actualiza un período de programa existente
    /// </summary>
    Task<bool> UpdateProgramPeriod(ProgramPeriodRequest request);

    /// <summary>
    /// Calcula automáticamente las fechas de inicio y fin para un programa y año
    /// </summary>
    /// <param name="programId">ID del programa</param>
    /// <param name="year">Año del período</param>
    /// <returns>Tupla con StartDate y EndDate</returns>
    (DateTime StartDate, DateTime EndDate) CalculateProgramPeriodDates(int programId, int year);
}

