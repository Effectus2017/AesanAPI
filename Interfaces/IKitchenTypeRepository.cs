using Api.Models;

namespace Api.Interfaces;

public interface IKitchenTypeRepository
{
    /// <summary>
    /// Obtiene un tipo de cocina por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de cocina a obtener.</param>
    /// <returns>El tipo de cocina encontrado o null si no se encuentra.</returns>
    Task<dynamic> GetKitchenTypeById(int id);

    /// <summary>
    /// Obtiene todos los tipos de cocina.
    /// </summary>
    /// <param name="take">El número de tipos a tomar.</param>
    /// <param name="skip">El número de tipos a saltar.</param>
    /// <param name="name">El nombre del tipo a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los tipos.</param>
    /// <returns>Una lista de tipos de cocina.</returns>
    Task<dynamic> GetAllKitchenTypes(int take, int skip, string name, bool alls);

    /// <summary>
    /// Inserta un nuevo tipo de cocina.
    /// </summary>
    /// <param name="kitchenType">El tipo de cocina a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertKitchenType(DTOKitchenType kitchenType);

    /// <summary>
    /// Actualiza un tipo de cocina existente.
    /// </summary>
    /// <param name="kitchenType">El tipo de cocina a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateKitchenType(DTOKitchenType kitchenType);

    /// <summary>
    /// Actualiza el orden de visualización de un tipo de cocina.
    /// </summary>
    /// <param name="kitchenTypeId">El ID del tipo a actualizar.</param>
    /// <param name="displayOrder">El nuevo orden de visualización.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateKitchenTypeDisplayOrder(int kitchenTypeId, int displayOrder);

    /// <summary>
    /// Elimina un tipo de cocina existente.
    /// </summary>
    /// <param name="id">El ID del tipo a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteKitchenType(int id);
}