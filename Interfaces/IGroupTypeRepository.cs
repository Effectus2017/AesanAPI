using Api.Models;

namespace Api.Interfaces;

public interface IGroupTypeRepository
{
    /// <summary>
    /// Obtiene un tipo de grupo por su ID
    /// </summary>
    /// <param name="id">ID del tipo de grupo</param>
    /// <returns>Tipo de grupo</returns>
    Task<dynamic> GetGroupTypeById(int id);

    /// <summary>
    /// Obtiene todos los tipos de grupo
    /// </summary>
    /// <param name="take">Número de tipos de grupo a obtener</param>
    /// <param name="skip">Número de tipos de grupo a saltar</param>
    /// <param name="name">Nombre del tipo de grupo a buscar</param>
    /// <param name="alls">Si se deben obtener todos los tipos de grupo</param>
    Task<dynamic> GetAllGroupTypes(int take, int skip, string name, bool alls, bool isList);

    /// <summary>
    /// Inserta un tipo de grupo
    /// </summary>
    /// <param name="groupType">Tipo de grupo a insertar</param>
    /// <returns>True si se insertó correctamente, false en caso contrario</returns>
    Task<bool> InsertGroupType(DTOGroupType groupType);

    /// <summary>
    /// Actualiza un tipo de grupo
    /// </summary>
    /// <param name="groupType">Tipo de grupo a actualizar</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    Task<bool> UpdateGroupType(DTOGroupType groupType);

    /// <summary>
    /// Actualiza el orden de visualización de un tipo de grupo
    /// </summary>
    /// <param name="groupTypeId">ID del tipo de grupo</param>
    /// <param name="displayOrder">Nuevo orden de visualización</param>
    Task<bool> UpdateGroupTypeDisplayOrder(int groupTypeId, int displayOrder);

    /// <summary>
    /// Elimina un tipo de grupo
    /// </summary>
    /// <param name="id">ID del tipo de grupo</param>
    /// <returns>True si se eliminó correctamente, false en caso contrario</returns>
    Task<bool> DeleteGroupType(int id);
}