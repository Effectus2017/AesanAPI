using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

public interface IEmailLogRepository
{
    Task<int> InsertEmailLog(EmailLogRequest request);
    Task<List<DTOEmailLog>> GetEmailLogsByUserId(string userId);
    Task<List<DTOEmailLog>> GetEmailLogsByEmail(string email);
    Task<DTOEmailLog?> GetEmailLogById(int id);
    Task<bool> UpdateEmailLogStatus(int id, string status, string? errorMessage = null);
    Task<List<DTOEmailLog>> GetFailedEmailLogs(string? email = null);
}
