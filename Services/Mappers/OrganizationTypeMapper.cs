using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con OrganizationType
/// Contiene todos los métodos de mapeo para OrganizationTypeResponse
/// </summary>
public static class OrganizationTypeMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de organización (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>OrganizationTypeResponse</returns>
    public static OrganizationTypeResponse MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new OrganizationTypeResponse();
            }

            return new OrganizationTypeResponse
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new OrganizationTypeResponse();
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de organización completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>OrganizationTypeResponse</returns>
    public static OrganizationTypeResponse MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new OrganizationTypeResponse();
            }

            return new OrganizationTypeResponse
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN,
                IsActive = result.IsActive,
                DisplayOrder = result.DisplayOrder,
                RequiresCenterType = result.RequiresCenterType
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new OrganizationTypeResponse();
        }
    }
}
