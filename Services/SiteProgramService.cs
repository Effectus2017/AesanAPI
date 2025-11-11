using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.Extensions.Logging;

namespace Api.Services;

/// <summary>
/// Servicio para gestionar relaciones sitio-programa
/// </summary>
public class SiteProgramService(
    ISiteProgramRepository repository,
    ILogger<SiteProgramService> logger) : ISiteProgramService
{
    private readonly ISiteProgramRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly ILogger<SiteProgramService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene todos los programas de un sitio
    /// </summary>
    public async Task<IEnumerable<SiteProgramResponse>> GetSiteProgramsBySiteId(int siteId)
    {
        try
        {
            return await _repository.GetSiteProgramsBySiteId(siteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener programas del sitio {SiteId}", siteId);
            throw;
        }
    }

    /// <summary>
    /// Inserta una nueva relación sitio-programa
    /// </summary>
    public async Task<SiteProgramResponse> InsertSiteProgram(SiteProgramRequest request)
    {
        try
        {
            var id = await _repository.InsertSiteProgram(request);
            var results = await _repository.GetSiteProgramsBySiteId(request.SiteId);
            var result = results.FirstOrDefault(sp => sp.Id == id);
            
            if (result == null)
            {
                throw new Exception("Error al recuperar la relación sitio-programa insertada");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar relación sitio-programa");
            throw;
        }
    }

    /// <summary>
    /// Actualiza una relación sitio-programa existente
    /// </summary>
    public async Task<bool> UpdateSiteProgram(SiteProgramRequest request)
    {
        try
        {
            return await _repository.UpdateSiteProgram(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar relación sitio-programa {Id}", request.Id);
            throw;
        }
    }
}

