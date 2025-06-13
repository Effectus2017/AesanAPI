using Api.Models;

namespace Api.Interfaces;

public interface ICenterTypeRepository
{

    /// <summary>
    /// Obtiene un tipo de centro por su ID
    /// </summary>
    /// <param name="id">El ID del tipo de centro a obtener</param>
    /// <returns>El tipo de centro encontrado</returns>
    Task<dynamic> GetCenterTypeById(int id);

    /// <summary>
    /// Obtiene todos los tipos de centro
    /// </summary>
    /// <param name="take">El número de tipos de centro a obtener</param>
    /// <param name="skip">El número de tipos de centro a saltar</param>
    /// <param name="name">Los nombres de los tipos de centro a buscar</param>
    /// <param name="alls">Si se deben obtener todos los tipos de centro</param>
    /// <returns>Los tipos de centro encontrados</returns>
    Task<dynamic> GetAllCenterTypes(int take, int skip, string name, bool alls, bool isList);

    /// <summary>
    /// Inserta un tipo de centro
    /// </summary>
    /// <param name="request">El tipo de centro a insertar</param>
    /// <returns>True si la inserción fue exitosa, false en caso contrario</returns>
    Task<bool> InsertCenterType(CenterTypeRequest request);

    /// <summary>
    /// Actualiza un tipo de centro existente
    /// </summary>
    /// <param name="request">El tipo de centro a actualizar</param>
    /// <returns>True si la actualización fue exitosa, false en caso contrario</returns>
    Task<bool> UpdateCenterType(DTOCenterType request);

    /// <summary>
    /// Elimina un tipo de centro
    /// </summary>
    /// <param name="id">El ID del tipo de centro a eliminar</param>
    /// <returns>True si la eliminación fue exitosa, false en caso contrario</returns>
    Task<bool> DeleteCenterType(int id);
}