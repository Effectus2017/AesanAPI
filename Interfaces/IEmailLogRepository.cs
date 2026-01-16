using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

public interface IEmailLogRepository
{
    Task<int> InsertEmailLogAsync(EmailLogRequest request);
    Task<List<DTOEmailLog>> GetEmailLogsByUserIdAsync(string userId);
    Task<List<DTOEmailLog>> GetEmailLogsByEmailAsync(string email);
    Task<DTOEmailLog?> GetEmailLogByIdAsync(int id);
    Task<bool> UpdateEmailLogStatusAsync(int id, string status, string? errorMessage = null);
    Task<List<DTOEmailLog>> GetFailedEmailLogsAsync(string? email = null);
}
