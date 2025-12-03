using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con Staff
/// Contiene todos los métodos de mapeo para DTOStaff y objetos relacionados
/// </summary>
public static class StaffMapper
{
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
                result.HasRelationships
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
                result.ReviewResultId,
                result.ReviewDate,
                result.ReviewJustification,

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
                ReviewResultId = item.ReviewResultId,
                ReviewDate = item.ReviewDate,
                ReviewJustification = item.ReviewJustification,

                // Datos de la relación SiteStaff
                SiteId = item.SiteId,
                IsPrimary = item.IsPrimary,

                Site = item.SiteId != null ? mappingService.MapSiteListItem(new { Id = item.SiteId, Name = item.SiteName, AgencyId = item.SiteAgencyId ?? 0 }) : null,

                City = mappingService.MapCity(new { Id = item.CityId ?? 0, Name = item.CityName ?? string.Empty }),
                Region = mappingService.MapRegion(new { Id = item.RegionId ?? 0, Name = item.RegionName ?? string.Empty }),
                Status = item.StatusId != null ? mappingService.MapOptionSelection(item.StatusId, item.StatusName, item.StatusNameEN) : null,
                Position = item.PositionId != null ? mappingService.MapOptionSelection(item.PositionId, item.PositionName, item.PositionNameEN) : null,
                StaffType = item.StaffTypeId != null ? mappingService.MapStaffType(item.StaffTypeId, item.StaffTypeName, item.StaffTypeNameEn) : null,
                StaffClassification = item.StaffClassificationId != null ? mappingService.MapStaffClassification(item.StaffClassificationId, item.StaffClassificationName, item.StaffClassificationNameEn) : null
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
