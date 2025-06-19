using System.Threading.Tasks;
using Api.Models;

namespace Api.Interfaces;

public interface IOptionSelectionRepository
{
    /// <summary>
    /// Obtiene una selección de opción por su ID
    /// </summary>
    /// <param name="id">El ID de la selección de opción</param>
    /// <returns>La selección de opción encontrada o null si no se encuentra</returns>
    Task<dynamic> GetOptionSelectionById(int id);

    /// <summary>
    /// Obtiene una selección de opción por su clave
    /// </summary>
    /// <param name="optionKey">La clave de la selección de opción</param>
    /// <returns>La selección de opción encontrada o null si no se encuentra</returns>
    Task<dynamic> GetOptionSelectionByOptionKey(string optionKey);

    /// <summary>
    /// Obtiene todas las selecciones de opciones
    /// </summary>
    /// <param name="take">El número de selecciones de opciones a tomar</param>
    /// <param name="skip">El número de selecciones de opciones a saltar</param>
    /// <param name="name">El nombre de la selección de opción</param>
    /// <param name="optionType">El tipo de opción</param>
    /// <param name="alls">Si se deben obtener todas las selecciones de opciones</param>
    /// <param name="isList">Si se debe devolver una lista o un objeto</param>
    /// <returns>La selección de opción encontrada o null si no se encuentra</returns>
    Task<dynamic> GetAllOptionSelections(int take, int skip, string name, string optionType, bool alls, bool isList);

    /// <summary>
    /// Inserta una nueva selección de opción
    /// </summary>
    /// <param name="optionSelection">La selección de opción a insertar</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario</returns>
    Task<bool> InsertOptionSelection(DTOOptionSelection optionSelection);

    /// <summary>
    /// Actualiza una selección de opción
    /// </summary>
    /// <param name="optionSelection">La selección de opción a actualizar</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario</returns>
    Task<bool> UpdateOptionSelection(DTOOptionSelection optionSelection);

    /// <summary>
    /// Actualiza el orden de visualización de una selección de opción
    /// </summary>
    /// <param name="optionSelectionId">El ID de la selección de opción</param>
    /// <param name="displayOrder">El orden de visualización</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario</returns>
    Task<bool> UpdateOptionSelectionDisplayOrder(int optionSelectionId, int displayOrder);

    /// <summary>
    /// Elimina una selección de opción
    /// </summary>
    /// <param name="id">El ID de la selección de opción</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario</returns>
    Task<bool> DeleteOptionSelection(int id);
}
