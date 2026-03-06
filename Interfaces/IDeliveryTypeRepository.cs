using Api.Models;
using Api.Models.Request;

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
    Task<dynamic> GetAllDeliveryTypes(int take, int skip, string name, bool alls, bool isList);

    /// <summary>
    /// Inserta un nuevo tipo de entrega.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertDeliveryType(DeliveryTypeRequest deliveryType);

    /// <summary>
    /// Actualiza un tipo de entrega existente.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateDeliveryType(DeliveryTypeRequest deliveryType);

    /// <summary>
    /// Elimina un tipo de entrega existente.
    /// </summary>
    /// <param name="id">El ID del tipo a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteDeliveryType(int id);

    /// <summary>
    /// Obtiene los tipos de entrega válidos para un programa específico.
    /// </summary>
    /// <param name="programId">El ID del programa.</param>
    /// <returns>Los tipos de entrega válidos para el programa.</returns>
    Task<dynamic> GetDeliveryTypesByProgram(int programId);

    /// <summary>
    /// Obtiene los tipos de entrega válidos para un tipo de grupo específico, opcionalmente filtrados por programa.
    /// </summary>
    /// <param name="groupTypeId">El ID del tipo de grupo.</param>
    /// <param name="programId">El ID del programa (opcional). Si se especifica, solo devuelve tipos de entrega válidos para ese programa.</param>
    /// <returns>Los tipos de entrega válidos para el tipo de grupo con información de RequiresPermission.</returns>
    Task<dynamic> GetDeliveryTypesByGroupType(int groupTypeId, int? programId = null);
}