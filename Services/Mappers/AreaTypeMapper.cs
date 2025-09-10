using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con AreaType
/// Contiene todos los métodos de mapeo para DTOAreaType
/// </summary>
public static class AreaTypeMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de área (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOAreaType</returns>
    public static DTOAreaType MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOAreaType();
            }

            return new DTOAreaType
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOAreaType();
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de área completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOAreaType</returns>
    public static DTOAreaType MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOAreaType();
            }

            return new DTOAreaType
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN,
                IsActive = result.IsActive,
                DisplayOrder = result.DisplayOrder
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOAreaType();
        }
    }
}
