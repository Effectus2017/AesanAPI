using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con StaffClassification
/// Contiene todos los métodos de mapeo para objetos de StaffClassification
/// </summary>
public class StaffClassificationMapper
{
    /// <summary>
    /// Mapea una clasificación de staff desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>Objeto mapeado para listas</returns>
    public static dynamic MapListFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new
            {
                item.Id,
                item.Name,
                item.NameEn,
                item.SortOrder,
                item.IsActive,
                item.CreatedAt,
                item.UpdatedAt
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la clasificación de staff para lista: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la clasificación de staff para lista: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea una clasificación de staff desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>Objeto mapeado para listas paginadas</returns>
    public static dynamic MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new
            {
                item.Id,
                item.Name,
                item.NameEn,
                item.SortOrder,
                item.IsActive,
                item.CreatedAt,
                item.UpdatedAt
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la clasificación de staff: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la clasificación de staff: {ex.Message}", ex);
        }
    }
}
