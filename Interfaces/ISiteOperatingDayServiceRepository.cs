using System.Data;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de servicios de alimentación por día de funcionamiento
/// Define las operaciones CRUD para la gestión de servicios relacionados con días operativos
/// </summary>
public interface ISiteOperatingDayServiceRepository
{
    /// <summary>
    /// Obtiene todos los servicios de un día de funcionamiento
    /// </summary>
    /// <param name="operatingDayId">ID del día de funcionamiento</param>
    /// <returns>Lista de servicios del día</returns>
    Task<List<SiteOperatingDayServiceResponse>> GetServicesByOperatingDay(int operatingDayId);

    /// <summary>
    /// Obtiene un servicio por su ID
    /// </summary>
    /// <param name="id">ID del servicio</param>
    /// <returns>Servicio encontrado o null</returns>
    Task<SiteOperatingDayServiceResponse?> GetServiceById(int id);

    /// <summary>
    /// Crea un nuevo servicio para un día de funcionamiento
    /// </summary>
    /// <param name="request">Datos del servicio a crear</param>
    /// <returns>ID del servicio creado</returns>
    Task<int> CreateService(SiteOperatingDayServiceRequest request);

    /// <summary>
    /// Crea múltiples servicios en batch para optimizar performance
    /// </summary>
    /// <param name="requests">Lista de servicios a crear</param>
    /// <returns>Número de servicios creados</returns>
    Task<int> CreateServicesBatch(List<SiteOperatingDayServiceRequest> requests, IDbConnection? connection = null, IDbTransaction? transaction = null);

    /// <summary>
    /// Actualiza un servicio existente
    /// </summary>
    /// <param name="id">ID del servicio</param>
    /// <param name="request">Datos actualizados del servicio</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> UpdateService(int id, SiteOperatingDayServiceRequest request);

    /// <summary>
    /// Elimina un servicio
    /// </summary>
    /// <param name="id">ID del servicio</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> DeleteService(int id);

    /// <summary>
    /// Habilita o deshabilita un servicio
    /// </summary>
    /// <param name="id">ID del servicio</param>
    /// <param name="isEnabled">Estado a establecer</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> ToggleService(int id, bool isEnabled);

    /// <summary>
    /// Valida que los horarios del servicio estén dentro del rango del día de funcionamiento
    /// </summary>
    /// <param name="operatingDayId">ID del día de funcionamiento</param>
    /// <param name="startTime">Hora de inicio del servicio</param>
    /// <param name="endTime">Hora de fin del servicio</param>
    /// <returns>True si los horarios son válidos</returns>
    Task<bool> ValidateServiceTimeRange(int operatingDayId, TimeSpan startTime, TimeSpan endTime);
}

