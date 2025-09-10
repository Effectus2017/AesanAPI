using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con CenterType
/// Contiene todos los métodos de mapeo para DTOCenterType
/// </summary>
public static class CenterTypeMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de centro (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOCenterType</returns>
    public static DTOCenterType MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOCenterType();
            }

            return new DTOCenterType
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOCenterType();
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de centro completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOCenterType</returns>
    public static DTOCenterType MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOCenterType();
            }

            return new DTOCenterType
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
            return new DTOCenterType();
        }
    }
}
