using Api.Models;

namespace Api.Interfaces;

public interface IMealTypeRepository
{
    /// <summary>
    /// Obtiene un tipo de comida por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de comida a obtener.</param>
    /// <returns>El tipo de comida encontrado o null si no se encuentra.</returns>
    Task<dynamic> GetMealTypeById(int id);

    /// <summary>
    /// Obtiene todos los tipos de comida.
    /// </summary>
    /// <param name="take">El número de tipos de comida a tomar.</param>
    /// <param name="skip">El número de tipos de comida a saltar.</param>
    /// <param name="name">El nombre del tipo de comida a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los tipos de comida.</param>
    /// <returns>Una lista de tipos de comida.</returns>
    Task<dynamic> GetAllMealTypes(int take, int skip, string name, bool alls);

    /// <summary>
    /// Inserta un nuevo tipo de comida.
    /// </summary>
    /// <param name="mealType">El tipo de comida a insertar.</param>
    /// <returns>El ID del tipo de comida insertado.</returns>
    Task<bool> InsertMealType(DTOMealType mealType);

    /// <summary>
    /// Actualiza un tipo de comida existente.
    /// </summary>
    /// <param name="mealType">El tipo de comida a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateMealType(DTOMealType mealType);

    /// <summary>
    /// Elimina un tipo de comida existente.
    /// </summary>
    /// <param name="id">El ID del tipo de comida a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteMealType(int id);
}