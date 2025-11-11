using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con SponsorType
/// Contiene todos los métodos de mapeo para objetos de SponsorType
/// </summary>
public class SponsorTypeMapper
{
    /// <summary>
    /// Mapea un tipo de auspiciador desde un resultado dinámico
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOSponsorType</returns>
    public static DTOSponsorType MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOSponsorType
            {
                Id = item.Id,
                Name = item.Name,
                NameEN = item.NameEN,
                IsActive = item.IsActive,
                DisplayOrder = item.DisplayOrder,
                SelectionNotification = item.SelectionNotification ?? false,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el tipo de auspiciador: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el tipo de auspiciador: {ex.Message}", ex);
        }
    }
}
