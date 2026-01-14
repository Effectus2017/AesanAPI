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
                Name = result.Name ?? string.Empty,
                Address = result.Address ?? string.Empty,
                Phone = result.Phone ?? string.Empty,
                Email = result.Email ?? string.Empty,
                IsActive = result.IsActive ?? false,
                // Campos nuevos
                AgencyAssignmentType = result.AgencyAssignmentType,
                RoleId = result.RoleId,
                RoleName = result.RoleName,
                AssignedDate = result.AssignedDate,
                AssignedBy = result.AssignedBy,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt ?? result.CreatedAt,
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOAgencyUser();
        }
    }
}
