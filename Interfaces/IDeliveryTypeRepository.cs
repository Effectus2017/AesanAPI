using Api.Models;

namespace Api.Interfaces;

public interface IDeliveryTypeRepository
{
    /// <summary>
    /// Obtiene un tipo de entrega por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de entrega a obtener.</param>
    /// <returns>El tipo de entrega encontrado o null si no se encuentra.</returns>
    Task<dynamic> GetDeliveryTypeById(int id);

    /// <summary>
    /// Obtiene todos los tipos de entrega.
    /// </summary>
    /// <param name="take">El número de tipos a tomar.</param>
    /// <param name="skip">El número de tipos a saltar.</param>
    /// <param name="name">El nombre del tipo a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los tipos.</param>
    /// <returns>Una lista de tipos de entrega y el total.</returns>
    Task<dynamic> GetAllDeliveryTypes(int take, int skip, string name, bool alls);

    /// <summary>
    /// Inserta un nuevo tipo de entrega.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertDeliveryType(DTODeliveryType deliveryType);

    /// <summary>
    /// Actualiza un tipo de entrega existente.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateDeliveryType(DTODeliveryType deliveryType);

    /// <summary>
    /// Elimina un tipo de entrega existente.
    /// </summary>
    /// <param name="id">El ID del tipo a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteDeliveryType(int id);
}