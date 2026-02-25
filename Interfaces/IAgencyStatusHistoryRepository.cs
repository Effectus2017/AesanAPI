using Api.Models;

namespace Api.Interfaces;

public interface IAgencyStatusHistoryRepository
{
    Task<(IReadOnlyList<DTOAgencyStatusHistory> Items, int TotalCount)> GetAgencyStatusHistoryPagedAsync(
        int agencyId,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize);
}
