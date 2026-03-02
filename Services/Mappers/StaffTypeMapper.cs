using Api.Models;
using Api.Models.Response;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con StaffType
/// Contiene todos los métodos de mapeo para objetos dinámicos de StaffType
/// </summary>
public static class StaffTypeMapper
{
    /// <summary>
    /// Mapea el resultado de la consulta a StaffTypeDropdownItemResponse (dropdowns/listas).
    /// </summary>
    public static StaffTypeDropdownItemResponse? MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
                return null;

            return new StaffTypeDropdownItemResponse
            {
                Id = result.Id,
                Name = result.Name ?? string.Empty,
                NameEn = result.NameEn,
                DisplayOrder = result.DisplayOrder ?? 0,
                IsActive = result.IsActive ?? true
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a StaffTypeDropdownItemResponse.
    /// </summary>
    public static StaffTypeDropdownItemResponse? MapFromResult(dynamic result)
    {
        return MapListFromResult(result);
    }
}
