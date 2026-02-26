using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de calendario de citas de agencias
/// Define las operaciones CRUD y específicas para las citas
/// </summary>
public interface IAgencyCalendarRepository
{
    /// <summary>
    /// Obtiene las citas de una agencia específica
    /// Opcionalmente filtra por mes y año
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <param name="month">Mes opcional para filtrar (1-12)</param>
    /// <param name="year">Año opcional para filtrar</param>
    /// <returns>Respuesta con información de la agencia y sus citas</returns>
    Task<AgencyCalendarResponse> GetAppointments(int agencyId, int? month = null, int? year = null);

    /// <summary>
    /// Crea una nueva cita de agencia
    /// </summary>
    /// <param name="request">Datos de la cita</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>ID de la cita construida, o null si falló</returns>
    Task<int?> CreateAppointment(AgencyAppointmentRequest request, string? userId);

    /// <summary>
    /// Actualiza una cita existente
    /// </summary>
    /// <param name="id">ID de la cita</param>
    /// <param name="request">Datos actualizados de la cita</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> UpdateAppointment(int id, AgencyAppointmentRequest request, string? userId);

    /// <summary>
    /// Elimina una cita
    /// </summary>
    /// <param name="id">ID de la cita</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> DeleteAppointment(int id, string? userId);
}
