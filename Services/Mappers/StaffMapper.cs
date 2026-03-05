using Api.Models;
using Api.Models.Response;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con Staff
/// Contiene todos los métodos de mapeo para DTOStaff y objetos relacionados
/// </summary>
public static class StaffMapper
{
    /// <summary>
    /// Mapea un resultado dinámico a StaffTableResponse (solo columnas visibles en la tabla).
    /// </summary>
    public static StaffTableResponse? MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
                return null;

            return new StaffTableResponse
            {
                Id = result.Id,
                FirstName = result.FirstName ?? string.Empty,
                MiddleName = result.MiddleName,
                FatherLastName = result.FatherLastName ?? string.Empty,
                MotherLastName = result.MotherLastName,
                StaffClassificationName = result.StaffClassificationName,
                PositionName = result.PositionName,
                StaffTypeName = result.StaffTypeName?.ToString(),
                StaffTypeNameEn = result.StaffTypeNameEn?.ToString(),
                BirthDate = result.BirthDate,
                Email = result.Email?.ToString(),
                HasRelationships = result.HasRelationships ?? false,
                IsActive = result.IsActive ?? false,
                Comments = result.Comments
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a StaffDropdownItemResponse (Id, Name, StaffType, IsActive) para dropdowns.
    /// </summary>
    public static StaffDropdownItemResponse? MapDropdownItemFromResult(dynamic result)
    {
        try
        {
            if (result == null)
                return null;

            return new StaffDropdownItemResponse
            {
                Id = result.Id,
                FirstName = result.FirstName?.ToString() ?? string.Empty,
                MiddleName = result.MiddleName?.ToString(),
                FatherLastName = result.FatherLastName?.ToString() ?? string.Empty,
                MotherLastName = result.MotherLastName?.ToString(),
                StaffTypeName = result.StaffTypeName?.ToString(),
                StaffTypeNameEn = result.StaffTypeNameEn?.ToString(),
                IsActive = result.IsActive ?? true
            };
        }
        catch (Exception)
        {
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
                // SalaryOriginIds y SalaryOrigins se rellenan en el repositorio desde el segundo result set (StaffSalaryOrigin)

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

    /// <summary>
    /// Convierte TimeSpan? o valor dinámico a string "HH:mm" o null.
    /// </summary>
    private static string? ScheduleFromDynamicToHHmm(object? value)
    {
        if (value == null) return null;
        if (value is TimeSpan ts) return $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}";
        if (value is string s && !string.IsNullOrWhiteSpace(s)) return s;
        return null;
    }

    /// <summary>
    /// Obtiene un valor de un diccionario (fila Dapper con alias lowercase). Para tipos valor devuelve default si falta.
    /// </summary>
    private static T? Get<T>(IDictionary<string, object> d, string key) where T : struct
    {
        if (!d.TryGetValue(key, out var v) || v == null || v == DBNull.Value) return null;
        return (T)Convert.ChangeType(v, typeof(T));
    }

    private static object? GetValue(IDictionary<string, object> d, string key)
    {
        if (!d.TryGetValue(key, out var v) || v == DBNull.Value) return null;
        return v;
    }

    /// <summary>
    /// Mapea un resultado dinámico de 100_GetStaffContractByClassificationByStaffId a DTO.
    /// El SP devuelve alias en lowercase; ScheduleFrom/ScheduleTo vienen como TIME (TimeSpan).
    /// </summary>
    public static DTOStaffContractByClassification MapContractByClassificationFromResult(dynamic row)
    {
        if (row == null) throw new ArgumentNullException(nameof(row));
        var d = (IDictionary<string, object>)row;
        return new DTOStaffContractByClassification
        {
            Id = Get<int>(d, "id").GetValueOrDefault(),
            StaffId = Get<int>(d, "staffid").GetValueOrDefault(),
            StaffClassificationId = Get<int>(d, "staffclassificationid").GetValueOrDefault(),
            StaffClassificationName = (string?)GetValue(d, "staffclassificationname") ?? "",
            StaffClassificationNameEn = (string?)GetValue(d, "staffclassificationnameen") ?? "",
            PositionId = Get<int>(d, "positionid").GetValueOrDefault(),
            PositionName = (string?)GetValue(d, "positionname") ?? "",
            PositionNameEn = (string?)GetValue(d, "positionnameen") ?? "",
            ContractStartDate = Get<DateTime>(d, "contractstartdate"),
            ContractEndDate = Get<DateTime>(d, "contractenddate"),
            ScheduleFrom = ScheduleFromDynamicToHHmm(GetValue(d, "schedulefrom")),
            ScheduleTo = ScheduleFromDynamicToHHmm(GetValue(d, "scheduleto")),
            CreatedAt = Get<DateTime>(d, "createdat").GetValueOrDefault(),
            UpdatedAt = Get<DateTime>(d, "updatedat"),
            IsActive = Get<bool>(d, "isactive").GetValueOrDefault(true)
        };
    }

    /// <summary>
    /// Mapea una lista de resultados dinámicos a lista de DTOs.
    /// </summary>
    public static List<DTOStaffContractByClassification> MapContractByClassificationList(IEnumerable<dynamic>? rows)
    {
        if (rows == null) return new List<DTOStaffContractByClassification>();
        return rows.Cast<dynamic>().Select(MapContractByClassificationFromResult).ToList();
    }
}
