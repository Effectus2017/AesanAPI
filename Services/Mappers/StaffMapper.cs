using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con Staff
/// Contiene todos los métodos de mapeo para DTOStaff y objetos relacionados
/// </summary>
public static class StaffMapper
{
    /// <summary>
    /// Parsea el string de IDs separados por coma (salida del SP en columna salaryoriginids) a lista de enteros.
    /// </summary>
    private static List<int>? ParseSalaryOriginIds(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var list = new List<int>();
        foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries))
            if (int.TryParse(part.Trim(), out var id))
                list.Add(id);
        return list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Obtiene SalaryOriginIds desde un resultado dinámico (columna salaryoriginids en lowercase). Devuelve null si no existe.
    /// </summary>
    private static List<int>? TryGetSalaryOriginIdsFromResult(dynamic? result)
    {
        if (result == null) return null;
        try
        {
            return ParseSalaryOriginIds((string?)result.salaryoriginids);
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
        {
            return null;
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un objeto con campos necesarios para listas de staff
    /// </summary>
    /// <param name="result">Resultado dinámico</param>
    /// <returns>Objeto con campos necesarios para listas</returns>
    public static dynamic MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return null;
            }

            return new
            {
                result.Id,
                result.FirstName,
                result.MiddleName,
                result.FatherLastName,
                result.MotherLastName,
                result.StatusName,
                result.StatusNameEN,
                result.PositionName,
                result.PositionNameEN,
                result.StaffTypeId,
                result.StaffTypeName,
                result.StaffTypeNameEn,
                result.StaffClassificationId,
                result.StaffClassificationName,
                result.StaffClassificationNameEn,
                result.ContractStartDate,
                result.ContractEndDate,
                result.Email,
                result.CityName,
                result.RegionName,
                result.AgencyId,
                result.AgencyName,
                result.UserName,
                result.IsActive,
                result.HasRelationships,
                result.IsSiteAdmin
            };
        }
        catch (Exception ex)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un objeto con campos completos de staff
    /// </summary>
    /// <param name="result">Resultado dinámico</param>
    /// <returns>Objeto con campos completos</returns>
    public static dynamic MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return null;
            }

            return new
            {
                result.Id,
                result.FirstName,
                result.MiddleName,
                result.FatherLastName,
                result.MotherLastName,
                result.StatusId,
                result.StatusName,
                result.StatusNameEN,
                result.PositionId,
                result.PositionName,
                result.PositionNameEN,
                result.StaffTypeId,
                result.StaffTypeName,
                result.StaffTypeNameEn,
                result.StaffClassificationId,
                result.StaffClassificationName,
                result.StaffClassificationNameEn,
                result.ContractStartDate,
                result.ContractEndDate,
                result.BirthDate,
                result.Email,
                result.PostalAddress,
                result.CityId,
                result.CityName,
                result.RegionId,
                result.RegionName,
                result.ZipCode,
                result.AgencyId,
                result.AgencyName,
                result.Comments,
                result.UserId,
                result.UserName,
                result.CreatedAt,
                result.UpdatedAt,
                result.IsActive,
                result.HasRelationships,
                result.IsSiteAdmin,
                result.ReviewResultId,
                result.ReviewDate,
                result.ReviewJustification,
                result.TenureDuration,
                result.TenureDurationUnitId,
                result.TenureDurationUnitName,
                result.TenureDurationUnitNameEN,
                result.ReceivesProgramSalaryId,
                result.ReceivesProgramSalaryName,
                result.ReceivesProgramSalaryNameEN,
                SalaryOriginIds = TryGetSalaryOriginIdsFromResult(result),

                // Datos de la relación SchoolStaff
                SchoolId = result.SchoolId,
                IsPrimary = result.IsPrimary
            };
        }
        catch (Exception ex)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a un DTOStaff con relaciones anidadas
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <param name="mappingService">Servicio de mapeo para relaciones</param>
    /// <returns>DTOStaff</returns>
    public static DTOStaff MapDetailsFromResult(dynamic item, MappingService mappingService)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOStaff
            {
                Id = item.Id,
                FirstName = item.FirstName ?? string.Empty,
                MiddleName = item.MiddleName,
                FatherLastName = item.FatherLastName ?? string.Empty,
                MotherLastName = item.MotherLastName ?? string.Empty,
                StatusId = item.StatusId ?? 0,
                StatusName = item.StatusName ?? string.Empty,
                PositionId = item.PositionId ?? 0,
                PositionName = item.PositionName ?? string.Empty,
                StaffTypeId = item.StaffTypeId ?? 0,
                StaffTypeName = item.StaffTypeName ?? string.Empty,
                StaffTypeNameEn = item.StaffTypeNameEn ?? string.Empty,
                StaffClassificationId = item.StaffClassificationId,
                StaffClassificationName = item.StaffClassificationName ?? string.Empty,
                StaffClassificationNameEn = item.StaffClassificationNameEn ?? string.Empty,
                ContractStartDate = item.ContractStartDate,
                ContractEndDate = item.ContractEndDate,
                BirthDate = item.BirthDate ?? DateTime.MinValue,
                Email = item.Email ?? string.Empty,
                PostalAddress = item.PostalAddress ?? string.Empty,
                CityId = item.CityId ?? 0,
                CityName = item.CityName ?? string.Empty,
                RegionId = item.RegionId ?? 0,
                RegionName = item.RegionName ?? string.Empty,
                ZipCode = item.ZipCode ?? string.Empty,
                AgencyId = item.AgencyId,
                AgencyName = item.AgencyName ?? string.Empty,
                Comments = item.Comments,
                UserId = item.UserId,
                UserName = item.UserName,
                CreatedAt = item.CreatedAt ?? DateTime.Now,
                UpdatedAt = item.UpdatedAt,
                IsActive = item.IsActive ?? true,
                IsSiteAdmin = item.IsSiteAdmin,
                ReviewResultId = item.ReviewResultId,
                ReviewDate = item.ReviewDate,
                ReviewJustification = item.ReviewJustification,
                TenureDuration = item.TenureDuration,
                TenureDurationUnitId = item.TenureDurationUnitId,
                TenureDurationUnitName = item.TenureDurationUnitName,
                TenureDurationUnitNameEN = item.TenureDurationUnitNameEN,
                ReceivesProgramSalaryId = item.ReceivesProgramSalaryId,
                ReceivesProgramSalaryName = item.ReceivesProgramSalaryName,
                ReceivesProgramSalaryNameEN = item.ReceivesProgramSalaryNameEN,
                SalaryOriginIds = TryGetSalaryOriginIdsFromResult(item),

                // Datos de la relación SiteStaff
                SiteId = item.SiteId,
                IsPrimary = item.IsPrimary,

                Site = item.SiteId != null ? mappingService.MapSiteListItem(new { Id = item.SiteId, Name = item.SiteName, AgencyId = item.SiteAgencyId ?? 0 }) : null,

                City = mappingService.MapCity(new { Id = item.CityId ?? 0, Name = item.CityName ?? string.Empty }),
                Region = mappingService.MapRegion(new { Id = item.RegionId ?? 0, Name = item.RegionName ?? string.Empty }),
                Status = item.StatusId != null ? mappingService.MapOptionSelection(item.StatusId, item.StatusName, item.StatusNameEN) : null,
                Position = item.PositionId != null ? mappingService.MapOptionSelection(item.PositionId, item.PositionName, item.PositionNameEN) : null,
                StaffType = item.StaffTypeId != null ? mappingService.MapStaffType(item.StaffTypeId, item.StaffTypeName, item.StaffTypeNameEn) : null,
                StaffClassification = item.StaffClassificationId != null ? mappingService.MapStaffClassification(item.StaffClassificationId, item.StaffClassificationName, item.StaffClassificationNameEn) : null,
                TenureDurationUnit = item.TenureDurationUnitId != null ? mappingService.MapOptionSelection(item.TenureDurationUnitId, item.TenureDurationUnitName, item.TenureDurationUnitNameEN) : null,
                ReceivesProgramSalary = item.ReceivesProgramSalaryId != null ? mappingService.MapOptionSelection(item.ReceivesProgramSalaryId, item.ReceivesProgramSalaryName, item.ReceivesProgramSalaryNameEN) : null
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear el staff: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear el staff: {ex.Message}", ex);
        }
    }
}
