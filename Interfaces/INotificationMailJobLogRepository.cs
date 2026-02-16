using Api.Models.Request;

namespace Api.Interfaces;

public interface INotificationMailJobLogRepository
{
    Task<int> InsertAsync(NotificationMailJobLogRequest request, CancellationToken cancellationToken = default);
}
