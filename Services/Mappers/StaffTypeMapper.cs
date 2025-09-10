using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con StaffType
/// Contiene todos los métodos de mapeo para objetos dinámicos de StaffType
/// </summary>
public static class StaffTypeMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a un objeto StaffType (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Objeto StaffType mapeado</returns>
    public static dynamic MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new { };
            }

            return new
            {
                result.Id,
                result.Name,
                result.NameEn,
                result.DisplayOrder,
                result.IsActive,
                result.CreatedAt,
                result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new { };
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un objeto StaffType completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Objeto StaffType mapeado</returns>
    public static dynamic MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new { };
            }

            return new
            {
                result.Id,
                result.Name,
                result.NameEn,
                result.DisplayOrder,
                result.IsActive,
                result.CreatedAt,
                result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new { };
        }
    }
}
