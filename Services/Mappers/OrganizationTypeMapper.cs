using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con OrganizationType
/// Contiene todos los métodos de mapeo para DTOOrganizationType
/// </summary>
public static class OrganizationTypeMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de organización (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOOrganizationType</returns>
    public static DTOOrganizationType MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOOrganizationType();
            }

            return new DTOOrganizationType
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOOrganizationType();
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de organización completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOOrganizationType</returns>
    public static DTOOrganizationType MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOOrganizationType();
            }

            return new DTOOrganizationType
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
            return new DTOOrganizationType();
        }
    }
}
