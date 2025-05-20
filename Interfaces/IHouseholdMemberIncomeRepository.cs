using Api.Models;

namespace Api.Interfaces;

public interface IHouseholdMemberIncomeRepository
{
    /// <summary>
    /// Obtiene todos los ingresos de un miembro del hogar
    /// </summary>
    /// <param name="take">El número de ingresos a tomar.</param>
    /// <param name="skip">El número de ingresos a saltar.</param>
    /// <param name="name">El nombre del ingreso a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los ingresos.</param>
    Task<dynamic> GetAllAsync(int take, int skip, string name, bool alls);

    /// <summary>
    /// Obtiene un ingreso de un miembro del hogar por su ID
    /// </summary>
    /// <param name="id">El ID del ingreso a obtener.</param>
    /// <returns>El ingreso encontrado.</returns>
    Task<dynamic> GetByIdAsync(int id);

    /// <summary>
    /// Inserta un nuevo ingreso de un miembro del hogar
    /// </summary>
    /// <param name="entity">El ingreso a insertar.</param>
    /// <returns>El ID del ingreso insertado.</returns>
    Task<int> InsertAsync(DTOHouseholdMemberIncome entity);

    /// <summary>
    /// Actualiza un ingreso de un miembro del hogar
    /// </summary>
    /// <param name="entity">El ingreso a actualizar.</param>
    /// <returns>True si el ingreso se actualizó correctamente, false en caso contrario.</returns>
    Task<bool> UpdateAsync(DTOHouseholdMemberIncome entity);

    /// <summary>
    /// Elimina un ingreso de un miembro del hogar
    /// </summary>
    /// <param name="id">El ID del ingreso a eliminar.</param>
    /// <returns>True si el ingreso se eliminó correctamente, false en caso contrario.</returns>
    Task<bool> DeleteAsync(int id);
}
