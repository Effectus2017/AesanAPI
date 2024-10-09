

namespace Api.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task SaveAsync();
}

