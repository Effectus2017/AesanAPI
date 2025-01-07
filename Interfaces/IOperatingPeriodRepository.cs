using Api.Models;

namespace Api.Interfaces;

public interface IOperatingPeriodRepository
{
    /// <summary>
    /// Obtiene un período operativo por su ID.
    /// </summary>
    /// <param name="id">El ID del período operativo a obtener.</param>
    /// <returns>El período operativo encontrado o null si no se encuentra.</returns>
    Task<dynamic> GetOperatingPeriodById(int id);

    /// <summary>
    /// Obtiene todos los períodos operativos.
    /// </summary>
    /// <param name="take">El número de períodos operativos a tomar.</param>
    /// <param name="skip">El número de períodos operativos a saltar.</param>
    /// <param name="name">El nombre del período operativo a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los períodos operativos.</param>
    /// <returns>Una lista de períodos operativos.</returns>
    Task<dynamic> GetAllOperatingPeriods(int take, int skip, string name, bool alls);

    /// <summary>
    /// Inserta un nuevo período operativo.
    /// </summary>
    /// <param name="operatingPeriod">El período operativo a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertOperatingPeriod(DTOOperatingPeriod operatingPeriod);

    /// <summary>
    /// Actualiza un período operativo existente.
    /// </summary>
    /// <param name="operatingPeriod">El período operativo a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateOperatingPeriod(DTOOperatingPeriod operatingPeriod);

    /// <summary>
    /// Elimina un período operativo existente.
    /// </summary>
    /// <param name="id">El ID del período operativo a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteOperatingPeriod(int id);
}