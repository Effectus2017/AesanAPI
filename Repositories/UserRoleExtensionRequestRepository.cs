using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class UserRoleExtensionRequestRepository(DapperContext context) : IUserRoleExtensionRequestRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<int> InsertAsync(string userId, string roleId, DateTime requestedValidTo, string? reason = null)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.String);
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@requestedValidTo", requestedValidTo.Date, DbType.Date);
        parameters.Add("@reason", reason, DbType.String);

        var result = await db.QueryFirstOrDefaultAsync<dynamic>("100_InsertUserRoleExtensionRequest", parameters, commandType: CommandType.StoredProcedure);
        if (result != null && result.id != null)
            return Convert.ToInt32(result.id);
        return 0;
    }

    public async Task<(List<UserRoleExtensionRequestDto> Rows, int Total)> GetAsync(string? statusFilter = "Pending", int take = 50, int skip = 0)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@statusFilter", statusFilter, DbType.String);
        parameters.Add("@take", take, DbType.Int32);
        parameters.Add("@skip", skip, DbType.Int32);

        var multi = await db.QueryMultipleAsync("100_GetUserRoleExtensionRequests", parameters, commandType: CommandType.StoredProcedure);
        var rows = (await multi.ReadAsync<UserRoleExtensionRequestDto>()).ToList();
        var total = await multi.ReadFirstOrDefaultAsync<int>();
        return (rows, total);
    }

    public async Task<bool> ApproveAsync(int id, DateTime? newValidTo, string? processedBy)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);
        parameters.Add("@newValidTo", newValidTo?.Date, DbType.Date);
        parameters.Add("@processedBy", processedBy, DbType.String);

        var result = await db.QueryFirstOrDefaultAsync<dynamic>("100_ApproveUserRoleExtensionRequest", parameters, commandType: CommandType.StoredProcedure);
        return result != null && result.success == 1;
    }

    public async Task<bool> RejectAsync(int id, string? processedBy)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);
        parameters.Add("@processedBy", processedBy, DbType.String);

        var result = await db.QueryFirstOrDefaultAsync<dynamic>("100_RejectUserRoleExtensionRequest", parameters, commandType: CommandType.StoredProcedure);
        return result != null && result.success == 1;
    }

    public async Task<UserRoleExtensionRequestDto?> GetByIdAsync(int id)
    {
        using IDbConnection db = _context.CreateConnection();
        const string sql = @"
            SELECT e.Id AS id, e.UserId AS userid, e.RoleId AS roleid, r.Name AS rolename,
                   e.RequestedValidTo AS requestedvalidto, e.Reason AS reason, e.Status AS status,
                   e.RequestedAt AS requestedat, e.ProcessedAt AS processedat, e.ProcessedBy AS processedby,
                   u.Email AS useremail, (s.FirstName + ' ' + ISNULL(s.FatherLastName, '')) AS username
            FROM UserRoleExtensionRequest e
            INNER JOIN AspNetUsers u ON u.Id = e.UserId
            INNER JOIN AspNetRoles r ON r.Id = e.RoleId
            LEFT JOIN Staff s ON s.UserId = e.UserId
            WHERE e.Id = @id";
        return await db.QueryFirstOrDefaultAsync<UserRoleExtensionRequestDto>(sql, new { id });
    }
}
