using Api.Models;

namespace Api.Interfaces;

public interface IFoodAuthorityRepository
{
    /// <summary>
    /// Obtiene una autoridad alimentaria por su ID.
    /// </summary>
    /// <param name="id">El ID de la autoridad alimentaria a obtener.</param>
    /// <returns>La autoridad alimentaria encontrada o null si no se encuentra.</returns>
    Task<dynamic> GetFoodAuthorityById(int id);

    /// <summary>
    /// Obtiene todas las autoridades alimentarias.
    /// </summary>
    /// <param name="take">El número de autoridades alimentarias a tomar.</param>
    /// <param name="skip">El número de autoridades alimentarias a saltar.</param>
    /// <param name="name">El nombre de la autoridad alimentaria a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las autoridades alimentarias.</param>
    /// <returns>Una lista de autoridades alimentarias.</returns>
    Task<dynamic> GetAllFoodAuthorities(int take, int skip, string name, bool alls);

    /// <summary>
    /// Inserta una nueva autoridad alimentaria.
    /// </summary>
    /// <param name="foodAuthority">La autoridad alimentaria a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertFoodAuthority(FoodAuthorityRequest request);

    /// <summary>
    /// Actualiza una autoridad alimentaria existente.
    /// </summary>
    /// <param name="foodAuthority">La autoridad alimentaria a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateFoodAuthority(DTOFoodAuthority foodAuthority);

    /// <summary>
    /// Elimina una autoridad alimentaria existente.
    /// </summary>
    /// <param name="id">El ID de la autoridad alimentaria a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteFoodAuthority(int id);
}