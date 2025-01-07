using Api.Models;

namespace Api.Interfaces;

public interface IOrganizationTypeRepository
{
    /// <summary>
    /// Obtiene un tipo de organización por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de organización a obtener.</param>
    /// <returns>El tipo de organización encontrado o null si no se encuentra.</returns>
    Task<dynamic> GetOrganizationTypeById(int id);

    /// <summary>
    /// Obtiene todos los tipos de organización.
    /// </summary>
    /// <param name="take">El número de tipos de organización a tomar.</param>
    /// <param name="skip">El número de tipos de organización a saltar.</param>
    /// <param name="name">El nombre del tipo de organización a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los tipos de organización.</param>
    /// <returns>Una lista de tipos de organización.</returns>
    Task<dynamic> GetAllOrganizationTypes(int take, int skip, string name, bool alls);

    /// <summary>
    /// Inserta un nuevo tipo de organización.
    /// </summary>
    /// <param name="organizationType">El tipo de organización a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertOrganizationType(DTOOrganizationType organizationType);

    /// <summary>
    /// Actualiza un tipo de organización existente.
    /// </summary>
    /// <param name="organizationType">El tipo de organización a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateOrganizationType(DTOOrganizationType organizationType);

    /// <summary>
    /// Elimina un tipo de organización existente.
    /// </summary>
    /// <param name="id">El ID del tipo de organización a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteOrganizationType(int id);
}