using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con AgencyUser
/// Contiene todos los métodos de mapeo para DTOAgencyUser
/// </summary>
public static class AgencyUserMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a un usuario de agencia (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOAgencyUser</returns>
    public static DTOAgencyUser MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOAgencyUser();
            }

            return new DTOAgencyUser
            {
                Id = result.Id,
                Name = result.Name
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOAgencyUser();
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un usuario de agencia completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOAgencyUser</returns>
    public static DTOAgencyUser MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOAgencyUser();
            }

            return new DTOAgencyUser
            {
                Id = result.Id,
                Name = result.Name,
                Address = result.Address,
                Phone = result.Phone,
                Email = result.Email,
                IsOwner = result.IsOwner,
                IsMonitor = result.IsMonitor,
                IsActive = result.IsActive,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt,
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOAgencyUser();
        }
    }
}
