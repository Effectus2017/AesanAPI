using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Dapper;
using Api.Models.Errors;

namespace Api.Repositories;

public class SiteExcursionRepository : ISiteExcursionRepository
{
    private readonly DapperContext _context;

    public SiteExcursionRepository(DapperContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<SiteExcursionResponse?> GetSiteExcursionById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryMultipleAsync("100_GetSiteExcursionById", parameters, commandType: CommandType.StoredProcedure);

            var excursion = await result.ReadFirstOrDefaultAsync<dynamic>();
            var excludedServices = result.Read<dynamic>().ToList();

            if (excursion == null)
            {
                return null;
            }

            return MapSiteExcursion(excursion, excludedServices);
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener la excursión con ID {id}", ex);
        }
    }

    public async Task<List<SiteExcursionResponse>> GetSiteExcursionsBySiteId(int siteId, bool includeInactive = false)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@includeInactive", includeInactive, DbType.Boolean);

            var result = await dbConnection.QueryMultipleAsync("100_GetSiteExcursionsBySiteId", parameters, commandType: CommandType.StoredProcedure);

            var excursions = result.Read<dynamic>().ToList();
            var excludedServices = result.Read<dynamic>().ToList();

            return MapSiteExcursions(excursions, excludedServices);
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener las excursiones del sitio {siteId}", ex);
        }
    }

    public async Task<List<SiteExcursionResponse>> GetSiteExcursionsByDateRange(int siteId, DateTime startDate, DateTime endDate, bool includeInactive = false)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@startDate", startDate.Date, DbType.Date);
            parameters.Add("@endDate", endDate.Date, DbType.Date);
            parameters.Add("@includeInactive", includeInactive, DbType.Boolean);

            var result = await dbConnection.QueryMultipleAsync("100_GetSiteExcursionsByDateRange", parameters, commandType: CommandType.StoredProcedure);

            var excursions = result.Read<dynamic>().ToList();
            var excludedServices = result.Read<dynamic>().ToList();

            return MapSiteExcursions(excursions, excludedServices);
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener las excursiones del sitio {siteId} en el rango de fechas", ex);
        }
    }

    public async Task<List<SiteExcursionResponse>> GetSiteExcursionsByChildGroupId(int siteId, int childGroupId, bool includeInactive = false)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@childGroupId", childGroupId, DbType.Int32);
            parameters.Add("@includeInactive", includeInactive, DbType.Boolean);

            var result = await dbConnection.QueryMultipleAsync("100_GetSiteExcursionsByChildGroupId", parameters, commandType: CommandType.StoredProcedure);

            var excursions = result.Read<dynamic>().ToList();
            var excludedServices = result.Read<dynamic>().ToList();

            return MapSiteExcursions(excursions, excludedServices);
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener las excursiones del grupo {childGroupId} del sitio {siteId}", ex);
        }
    }

    public async Task<int> InsertSiteExcursion(SiteExcursionRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@childGroupId", request.ChildGroupId, DbType.Int32);
            parameters.Add("@activityDescription", request.ActivityDescription, DbType.String);
            parameters.Add("@excursionDate", request.ExcursionDate.Date, DbType.Date);
            parameters.Add("@isFullDay", request.IsFullDay, DbType.Boolean);
            parameters.Add("@isUnforeseen", request.IsUnforeseen, DbType.Boolean);
            parameters.Add("@comment", request.Comment, DbType.String);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSiteExcursion", parameters, commandType: CommandType.StoredProcedure);

            int excursionId = parameters.Get<int>("@id");

            // Insertar servicios excluidos
            if (request.ExcludedServiceTypeIds != null && request.ExcludedServiceTypeIds.Any())
            {
                await InsertExcludedServices(dbConnection, excursionId, request.ExcludedServiceTypeIds);
            }

            return excursionId;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al insertar la excursión", ex);
        }
    }

    public async Task<bool> UpdateSiteExcursion(SiteExcursionRequest request)
    {
        try
        {
            if (!request.Id.HasValue)
            {
                throw new ArgumentException("El ID de la excursión es requerido para actualizar");
            }

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", request.Id.Value, DbType.Int32);
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@childGroupId", request.ChildGroupId, DbType.Int32);
            parameters.Add("@activityDescription", request.ActivityDescription, DbType.String);
            parameters.Add("@excursionDate", request.ExcursionDate.Date, DbType.Date);
            parameters.Add("@isFullDay", request.IsFullDay, DbType.Boolean);
            parameters.Add("@isUnforeseen", request.IsUnforeseen, DbType.Boolean);
            parameters.Add("@comment", request.Comment, DbType.String);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);

            await dbConnection.ExecuteAsync("100_UpdateSiteExcursion", parameters, commandType: CommandType.StoredProcedure);

            // Eliminar servicios excluidos existentes y insertar los nuevos
            await DeleteExcludedServices(dbConnection, request.Id.Value);
            if (request.ExcludedServiceTypeIds != null && request.ExcludedServiceTypeIds.Any())
            {
                await InsertExcludedServices(dbConnection, request.Id.Value, request.ExcludedServiceTypeIds);
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al actualizar la excursión", ex);
        }
    }

    public async Task<bool> DeleteSiteExcursion(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            await dbConnection.ExecuteAsync("100_DeleteSiteExcursion", parameters, commandType: CommandType.StoredProcedure);

            return true;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al eliminar la excursión", ex);
        }
    }

    private async Task InsertExcludedServices(IDbConnection dbConnection, int excursionId, List<int> serviceTypeIds)
    {
        foreach (var serviceTypeId in serviceTypeIds)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteExcursionId", excursionId, DbType.Int32);
            parameters.Add("@serviceTypeId", serviceTypeId, DbType.Int32);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSiteExcursionExcludedService", parameters, commandType: CommandType.StoredProcedure);
        }
    }

    private async Task DeleteExcludedServices(IDbConnection dbConnection, int excursionId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@siteExcursionId", excursionId, DbType.Int32);

        await dbConnection.ExecuteAsync("100_DeleteSiteExcursionExcludedServicesByExcursionId", parameters, commandType: CommandType.StoredProcedure);
    }

    private SiteExcursionResponse MapSiteExcursion(dynamic excursion, List<dynamic> excludedServices)
    {
        return new SiteExcursionResponse
        {
            Id = excursion.Id,
            SiteId = excursion.SiteId,
            SiteName = excursion.SiteName,
            AgencyName = excursion.AgencyName,
            AgencyCode = excursion.AgencyCode,
            ChildGroupId = excursion.ChildGroupId,
            ChildGroupName = excursion.ChildGroupName,
            ActivityDescription = excursion.ActivityDescription,
            ExcursionDate = excursion.ExcursionDate,
            IsFullDay = excursion.IsFullDay,
            IsUnforeseen = excursion.IsUnforeseen,
            Comment = excursion.Comment,
            IsActive = excursion.IsActive,
            CreatedAt = excursion.CreatedAt,
            UpdatedAt = excursion.UpdatedAt,
            ExcludedServices = excludedServices.Select(MapExcludedService).ToList()
        };
    }

    private List<SiteExcursionResponse> MapSiteExcursions(List<dynamic> excursions, List<dynamic> excludedServices)
    {
        var result = excursions.Select(excursion => new SiteExcursionResponse
        {
            Id = excursion.Id,
            SiteId = excursion.SiteId,
            SiteName = excursion.SiteName,
            AgencyName = excursion.AgencyName,
            AgencyCode = excursion.AgencyCode,
            ChildGroupId = excursion.ChildGroupId,
            ChildGroupName = excursion.ChildGroupName,
            ActivityDescription = excursion.ActivityDescription,
            ExcursionDate = excursion.ExcursionDate,
            IsFullDay = excursion.IsFullDay,
            IsUnforeseen = excursion.IsUnforeseen,
            Comment = excursion.Comment,
            IsActive = excursion.IsActive,
            CreatedAt = excursion.CreatedAt,
            UpdatedAt = excursion.UpdatedAt,
            ExcludedServices = new List<SiteExcursionExcludedServiceResponse>()
        }).ToList();

        // Agrupar servicios excluidos por excursión
        var servicesByExcursion = excludedServices.GroupBy(s => s.SiteExcursionId);
        foreach (var group in servicesByExcursion)
        {
            var excursion = result.FirstOrDefault(e => e.Id == group.Key);
            if (excursion != null)
            {
                excursion.ExcludedServices = group.Select(MapExcludedService).ToList();
            }
        }

        return result;
    }

    private SiteExcursionExcludedServiceResponse MapExcludedService(dynamic service)
    {
        return new SiteExcursionExcludedServiceResponse
        {
            Id = service.Id,
            SiteExcursionId = service.SiteExcursionId,
            ServiceTypeId = service.ServiceTypeId,
            ServiceTypeName = service.ServiceTypeName,
            ServiceTypeNameEN = service.ServiceTypeNameEN,
            DisplayOrder = service.DisplayOrder,
            CreatedAt = service.CreatedAt
        };
    }
}

