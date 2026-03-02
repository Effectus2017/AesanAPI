using Api.Models;
using Api.Models.Response;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con StaffClassification
/// Contiene todos los m?todos de mapeo para objetos de StaffClassification
/// </summary>
public class StaffClassificationMapper
{
    /// <summary>
    /// Mapea una clasificaci?n de staff desde un resultado din?mico a StaffClassificationDropdownItemResponse (dropdowns/listas).
    /// </summary>
    public static StaffClassificationDropdownItemResponse MapListFromResult(dynamic item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");

        return new StaffClassificationDropdownItemResponse
        {
            Id = item.Id,
            Name = item.Name ?? string.Empty,
            NameEn = item.NameEn,
            SortOrder = item.SortOrder ?? 0,
            IsActive = item.IsActive ?? true
        };
    }

    /// <summary>
    /// Mapea una clasificaci?n de staff desde un resultado din?mico a StaffClassificationDropdownItemResponse.
    /// </summary>
    public static StaffClassificationDropdownItemResponse MapFromResult(dynamic item)
    {
        return MapListFromResult(item);
    }
}
