using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con AgencyStatus
/// Contiene todos los métodos de mapeo para DTOAgencyStatus
/// </summary>
public static class AgencyStatusMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a un estado de agencia
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOAgencyStatus</returns>
    public static DTOAgencyStatus MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOAgencyStatus();
            }

            return new DTOAgencyStatus
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOAgencyStatus();
        }
    }
}
