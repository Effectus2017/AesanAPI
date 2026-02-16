using Api.Models;

namespace Api.Interfaces;

/// <summary>Repositorio para solicitudes de extensión de vigencia de roles secundarios.</summary>
public interface IUserRoleExtensionRequestRepository
{
    Task<int> InsertAsync(string userId, string roleId, DateTime requestedValidTo, string? reason = null);
    Task<(List<UserRoleExtensionRequestDto> Rows, int Total)> GetAsync(string? statusFilter = "Pending", int take = 50, int skip = 0);
    Task<bool> ApproveAsync(int id, DateTime? newValidTo, string? processedBy);
    Task<bool> RejectAsync(int id, string? processedBy);
    Task<UserRoleExtensionRequestDto?> GetByIdAsync(int id);
}
