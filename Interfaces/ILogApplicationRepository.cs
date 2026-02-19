using Api.Models.Request;

namespace Api.Interfaces;

/// <summary>
/// Repositorio para la tabla LogApplication (errores de aplicación del sistema central).
/// </summary>
public interface ILogApplicationRepository
{
    Task<long> InsertAsync(LogApplicationRequest request, CancellationToken cancellationToken = default);
}
