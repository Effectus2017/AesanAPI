using Api.Models;

namespace Api.Interfaces;

public interface IOperatingPolicyRepository
{
    /// <summary>
    /// Obtiene una política operativa por su ID.
    /// </summary>
    /// <param name="id">El ID de la política operativa a obtener.</param>
    /// <returns>La política operativa encontrada o null si no se encuentra.</returns>
    Task<dynamic> GetOperatingPolicyById(int id);

    /// <summary>
    /// Obtiene todas las políticas operativas.
    /// </summary>
    /// <param name="take">El número de políticas operativas a tomar.</param>
    /// <param name="skip">El número de políticas operativas a saltar.</param>
    /// <param name="description">La descripción de la política operativa a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las políticas operativas.</param>
    /// <returns>Una lista de políticas operativas.</returns>
    Task<dynamic> GetAllOperatingPolicies(int take, int skip, string description, bool alls);

    /// <summary>
    /// Inserta una nueva política operativa.
    /// </summary>
    /// <param name="operatingPolicy">La política operativa a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertOperatingPolicy(DTOOperatingPolicy operatingPolicy);

    /// <summary>
    /// Actualiza una política operativa existente.
    /// </summary>
    /// <param name="operatingPolicy">La política operativa a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateOperatingPolicy(DTOOperatingPolicy operatingPolicy);

    /// <summary>
    /// Elimina una política operativa existente.
    /// </summary>
    /// <param name="id">El ID de la política operativa a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteOperatingPolicy(int id);
}