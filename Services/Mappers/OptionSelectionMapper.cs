using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con OptionSelection
/// Contiene todos los métodos de mapeo para DTOOptionSelection
/// </summary>
public static class OptionSelectionMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a una selección de opción (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOOptionSelection</returns>
    public static DTOOptionSelection MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOOptionSelection
                {
                    Id = 0,
                    Name = string.Empty,
                    NameEN = string.Empty,
                    OptionKey = string.Empty
                };
            }

            return new DTOOptionSelection
            {
                Id = result.Id,
                Name = result.Name ?? string.Empty,
                NameEN = result.NameEN ?? string.Empty,
                OptionKey = result.OptionKey ?? string.Empty,
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOOptionSelection
            {
                Id = 0,
                Name = string.Empty,
                NameEN = string.Empty,
                OptionKey = string.Empty
            };
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a una selección de opción completa
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOOptionSelection</returns>
    public static DTOOptionSelection MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOOptionSelection
                {
                    Id = 0,
                    Name = string.Empty,
                    NameEN = string.Empty,
                    OptionKey = string.Empty
                };
            }

            return new DTOOptionSelection
            {
                Id = result.Id,
                Name = result.Name ?? string.Empty,
                NameEN = result.NameEN ?? string.Empty,
                OptionKey = result.OptionKey ?? string.Empty,
                IsActive = result.IsActive,
                DisplayOrder = result.DisplayOrder,
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOOptionSelection
            {
                Id = 0,
                Name = string.Empty,
                NameEN = string.Empty,
                OptionKey = string.Empty
            };
        }
    }
}
