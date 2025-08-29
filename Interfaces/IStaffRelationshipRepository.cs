using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de relaciones entre empleados
/// </summary>
public interface IStaffRelationshipRepository
{
    /// <summary>
    /// Obtiene todas las relaciones de un empleado específico
    /// </summary>
    /// <param name="staffId">ID del empleado</param>
    /// <returns>Lista de relaciones del empleado</returns>
    Task<dynamic> GetRelationshipsByStaffId(int staffId, bool isActive);

    /// <summary>
    /// Obtiene una relación específica por su ID
    /// </summary>
    /// <param name="id">ID de la relación</param>
    /// <returns>Relación encontrada o null</returns>
    Task<dynamic> GetRelationshipById(int id);

    /// <summary>
    /// Crea una nueva relación entre empleados
    /// </summary>
    /// <param name="request">Datos de la relación a crear</param>
    /// <returns>ID de la relación creada</returns>
    Task<int> CreateRelationship(StaffRelationshipRequest request);

    /// <summary>
    /// Actualiza una relación existente
    /// </summary>
    /// <param name="request">Datos de la relación a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateRelationship(UpdateStaffRelationshipRequest request);

    /// <summary>
    /// Desactiva una relación (soft delete)
    /// </summary>
    /// <param name="id">ID de la relación a desactivar</param>
    /// <returns>True si se desactivó correctamente</returns>
    Task<bool> DeleteRelationship(int id);

    /// <summary>
    /// Verifica si existe una relación activa entre dos empleados
    /// </summary>
    /// <param name="staffId">ID del primer empleado</param>
    /// <param name="relatedStaffId">ID del segundo empleado</param>
    /// <returns>True si existe una relación activa</returns>
    Task<bool> RelationshipExists(int staffId, int relatedStaffId);

    /// <summary>
    /// Obtiene todas las relaciones activas de la agencia
    /// </summary>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <param name="alls">Si se deben obtener todos los registros</param>
    /// <param name="isList">Si es para lista simple</param>
    /// <returns>Lista de todas las relaciones activas</returns>
    Task<dynamic> GetAllActiveRelationshipsFromDb(int take, int skip, bool alls, bool isList);

    /// <summary>
    /// Obtiene las relaciones por tipo específico
    /// </summary>
    /// <param name="relationshipTypeId">ID del tipo de parentesco</param>
    /// <returns>Lista de relaciones del tipo especificado</returns>
    Task<dynamic> GetRelationshipsByType(int relationshipTypeId);

    /// <summary>
    /// Verifica si un empleado puede tener el tipo de relación especificado
    /// </summary>
    /// <param name="staffId">ID del empleado</param>
    /// <param name="relationshipTypeId">ID del tipo de parentesco</param>
    /// <param name="excludeRelationshipId">ID de la relación a excluir (útil para ediciones)</param>
    /// <returns>True si puede tener ese tipo de relación</returns>
    Task<bool> CanHaveRelationshipType(int staffId, int relationshipTypeId, int? excludeRelationshipId = null);
}
