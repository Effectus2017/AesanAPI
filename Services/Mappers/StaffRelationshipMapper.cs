using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con StaffRelationship
/// Contiene todos los métodos de mapeo para objetos de StaffRelationship
/// </summary>
public class StaffRelationshipMapper
{
    /// <summary>
    /// Mapea una relación de staff desde un resultado dinámico
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOStaffRelationship</returns>
    public static DTOStaffRelationship MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOStaffRelationship
            {
                Id = item.Id,
                Staff = new DTOStaffForRelationship
                {
                    Id = item.StaffId,
                    FullName = item.StaffFullName,
                    Position = item.StaffPosition,
                    StaffType = item.StaffType,
                    Email = item.StaffEmail,
                    IsActive = item.StaffIsActive
                },
                RelatedStaff = new DTOStaffForRelationship
                {
                    Id = item.RelatedStaffId,
                    FullName = item.RelatedStaffFullName,
                    Position = item.RelatedStaffPosition,
                    StaffType = item.RelatedStaffType,
                    Email = item.RelatedStaffEmail,
                    IsActive = item.RelatedStaffIsActive
                },
                RelationshipTypeId = item.RelationshipTypeId,
                RelationshipType = item.RelationshipType,
                RelationshipTypeEn = item.RelationshipTypeEn,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Comment = item.Comment
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la relación de staff: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la relación de staff: {ex.Message}", ex);
        }
    }
}
