using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con DeliveryType
/// Contiene todos los métodos de mapeo para DTODeliveryType
/// </summary>
public static class DeliveryTypeMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de entrega (versión simplificada para listas)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTODeliveryType</returns>
    public static DTODeliveryType MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTODeliveryType();
            }

            return new DTODeliveryType
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTODeliveryType();
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de entrega completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTODeliveryType</returns>
    public static DTODeliveryType MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTODeliveryType();
            }

            return new DTODeliveryType
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
            return new DTODeliveryType();
        }
    }
}
