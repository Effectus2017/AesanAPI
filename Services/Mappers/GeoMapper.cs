using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con datos geográficos (City, Region)
/// Contiene todos los métodos de mapeo para DTOCity y DTORegion
/// </summary>
public static class GeoMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a una ciudad
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOCity</returns>
    public static DTOCity MapCityFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOCity();
            }

            return new DTOCity
            {
                Id = result.Id,
                Name = result.Name ?? string.Empty
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la ciudad: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la ciudad: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a una región
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTORegion</returns>
    public static DTORegion MapRegionFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTORegion();
            }

            return new DTORegion
            {
                Id = result.Id,
                Name = result.Name ?? string.Empty
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la región: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la región: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a una ciudad (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOCity</returns>
    public static DTOCity MapCityListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOCity();
            }

            return new DTOCity
            {
                Id = result.Id,
                Name = result.Name ?? string.Empty
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la ciudad para lista: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la ciudad para lista: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a una región (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTORegion</returns>
    public static DTORegion MapRegionListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTORegion();
            }

            return new DTORegion
            {
                Id = result.Id,
                Name = result.Name ?? string.Empty
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la región para lista: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la región para lista: {ex.Message}", ex);
        }
    }
}
