using Api.Models.Response;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con DeliveryType
/// Contiene todos los métodos de mapeo para DeliveryTypeResponse
/// </summary>
public static class DeliveryTypeMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de entrega (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DeliveryTypeResponse</returns>
    public static DeliveryTypeResponse MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DeliveryTypeResponse();
            }

            return new DeliveryTypeResponse
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN,
                RequiresPermission = result.RequiresPermission,
                DisplayOrder = result.DisplayOrder,
                IsActive = result.IsActive,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DeliveryTypeResponse();
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de entrega completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DeliveryTypeResponse</returns>
    public static DeliveryTypeResponse MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DeliveryTypeResponse();
            }

            return new DeliveryTypeResponse
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN,
                IsActive = result.IsActive,
                DisplayOrder = result.DisplayOrder,
                RequiresPermission = result.RequiresPermission,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DeliveryTypeResponse();
        }
    }
}
