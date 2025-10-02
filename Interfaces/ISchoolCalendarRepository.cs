using Api.Models;
using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de calendario de funcionamiento de escuelas
/// Define las operaciones CRUD y específicas para la gestión de días de funcionamiento
/// </summary>
public interface ISchoolCalendarRepository
{
    /// <summary>
    /// Obtiene todos los días de funcionamiento de una escuela específica
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <returns>Respuesta con información de la escuela y sus días de funcionamiento</returns>
    Task<SchoolCalendarResponse> GetOperatingDays(int schoolId);

    /// <summary>
    /// Alterna el estado de funcionamiento de un día específico
    /// Inserta o actualiza un día de funcionamiento
    /// </summary>
    /// <param name="request">Datos del día de funcionamiento</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> ToggleOperatingDay(SchoolOperatingDayRequest request);

    /// <summary>
    /// Actualiza un día de funcionamiento existente
    /// </summary>
    /// <param name="id">ID del día de funcionamiento</param>
    /// <param name="request">Datos actualizados del día de funcionamiento</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> UpdateOperatingDay(int id, SchoolOperatingDayRequest request);

    /// <summary>
    /// Actualiza múltiples días de funcionamiento en lote
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="requests">Lista de días de funcionamiento a actualizar</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> BulkUpdateOperatingDays(int schoolId, List<SchoolOperatingDayRequest> requests);


    /// <summary>
    /// Elimina un día de funcionamiento
    /// </summary>
    /// <param name="id">ID del día de funcionamiento</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> DeleteOperatingDay(int id);

    /// <summary>
    /// Verifica si una escuela existe
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <returns>True si la escuela existe</returns>
    Task<bool> SchoolExists(int schoolId);
}
