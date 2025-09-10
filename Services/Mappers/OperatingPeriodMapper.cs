using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con OperatingPeriod
/// Contiene todos los métodos de mapeo para objetos de OperatingPeriod
/// </summary>
public class OperatingPeriodMapper
{
    /// <summary>
    /// Mapea un período operativo desde un resultado dinámico para listas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOperatingPeriod para listas</returns>
    public static DTOOperatingPeriod MapListFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOOperatingPeriod
            {
                Id = item.Id,
                Name = item.Name,
                NameEN = item.NameEN,
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el período operativo para lista: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el período operativo para lista: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea un período operativo desde un resultado dinámico para listas paginadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOOperatingPeriod para listas paginadas</returns>
    public static DTOOperatingPeriod MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOOperatingPeriod
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
            throw new InvalidOperationException($"Error al mapear el período operativo: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el período operativo: {ex.Message}", ex);
        }
    }
}
