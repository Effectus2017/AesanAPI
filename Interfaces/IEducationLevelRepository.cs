using Api.Models;

namespace Api.Interfaces;

public interface IEducationLevelRepository
{
    /// <summary>
    /// Obtiene un nivel educativo por su ID.
    /// </summary>
    /// <param name="id">El ID del nivel educativo a obtener.</param>
    /// <returns>El nivel educativo encontrado o null si no se encuentra.</returns>
    Task<dynamic> GetEducationLevelById(int id);

    /// <summary>
    /// Obtiene todos los niveles educativos.
    /// </summary>
    /// <param name="take">El número de niveles educativos a tomar.</param>
    /// <param name="skip">El número de niveles educativos a saltar.</param>
    /// <param name="name">El nombre del nivel educativo a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los niveles educativos.</param>
    /// <returns>Una lista de niveles educativos.</returns>
    Task<dynamic> GetAllEducationLevels(int take, int skip, string name, bool alls, bool isList);

    /// <summary>
    /// Inserta un nuevo nivel educativo.
    /// </summary>
    /// <param name="educationLevel">El nivel educativo a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertEducationLevel(DTOEducationLevel educationLevel);

    /// <summary>
    /// Actualiza un nivel educativo existente.
    /// </summary>
    /// <param name="educationLevel">El nivel educativo a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateEducationLevel(DTOEducationLevel educationLevel);

    /// <summary>
    /// Elimina un nivel educativo existente.
    /// </summary>
    /// <param name="id">El ID del nivel educativo a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteEducationLevel(int id);
}