using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con OperatingPolicy
/// Contiene todos los métodos de mapeo para objetos de OperatingPolicy
/// </summary>
public class OperatingPolicyMapper
{
    /// <summary>
    /// Mapea una política operativa desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOperatingPolicy para listas</returns>
    public static DTOOperatingPolicy MapListFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOOperatingPolicy
            {
                Id = item.Id,
                Name = item.Name,
                NameEN = item.NameEN,
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la política operativa para lista: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la política operativa para lista: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea una política operativa desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOperatingPolicy para listas paginadas</returns>
    public static DTOOperatingPolicy MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOOperatingPolicy
            {
                Id = item.Id,
                Name = item.Name,
                NameEN = item.NameEN,
                IsActive = item.IsActive,
                DisplayOrder = item.DisplayOrder,
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la política operativa: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la política operativa: {ex.Message}", ex);
        }
    }
}
