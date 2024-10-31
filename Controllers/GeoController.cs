using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("geo")]
public class GeoController(ILogger<GeoController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<GeoController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    [HttpGet("get-all-cities-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las ciudades de la base de datos", Description = "Devuelve una lista de todas las ciudades.")]
    public async Task<IActionResult> GetCities([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var cities = await _unitOfWork.GeoRepository.GetAllCitiesFromDb(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(cities);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las ciudades");
            return StatusCode(500, "Error al obtener las ciudades");
        }
    }

    [HttpGet("get-all-regions-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las regiones de la base de datos", Description = "Devuelve una lista de todas las regiones.")]
    public async Task<IActionResult> GetAllRegions([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var regions = await _unitOfWork.GeoRepository.GetAllRegionsFromDb(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
            return Ok(regions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las regiones");
            return StatusCode(500, "Error al obtener las regiones");
        }
    }

    [HttpGet("get-regions-by-city-id")]
    [SwaggerOperation(Summary = "Obtiene todas las regiones por ID de ciudad", Description = "Devuelve una lista de todas las regiones por ID de ciudad.")]
    public async Task<IActionResult> GetRegionsByCityId([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            _logger.LogInformation("Obteniendo regiones por ID de ciudad: {Id}", queryParameters.CityId);

            if (queryParameters.CityId == 0)
            {
                return BadRequest("El ID de la ciudad es requerido");
            }

            var regions = await _unitOfWork.GeoRepository.GetRegionsByCityId(queryParameters.CityId ?? 0);
            if (regions == null)
            {
                return NotFound();
            }
            return Ok(regions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las regiones por ID de ciudad");
            return StatusCode(500, "Error al obtener las regiones por ID de ciudad");
        }
    }

    [HttpGet("get-city-by-id")]
    [SwaggerOperation(Summary = "Obtiene una ciudad por su ID", Description = "Devuelve una ciudad basada en el ID proporcionado.")]
    public async Task<IActionResult> GetCityById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            _logger.LogInformation("Obteniendo ciudad por ID: {Id}", queryParameters.CityId);
            if (queryParameters.CityId == 0)
            {
                return BadRequest("El ID de la ciudad es requerido");
            }

            var city = await _unitOfWork.GeoRepository.GetCityById(queryParameters.CityId ?? 0);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la ciudad por ID");
            return StatusCode(500, "Error al obtener la ciudad por ID");
        }
    }

    [HttpGet("get-region-by-id")]
    [SwaggerOperation(Summary = "Obtiene una región por su ID", Description = "Devuelve una región basada en el ID proporcionado.")]
    public async Task<IActionResult> GetRegionById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            _logger.LogInformation("Obteniendo región por ID: {Id}", queryParameters.RegionId);

            if (queryParameters.RegionId == 0)
            {
                return BadRequest("El ID de la región es requerido");
            }

            var region = await _unitOfWork.GeoRepository.GetRegionById(queryParameters.RegionId ?? 0);
            if (region == null)
            {
                return NotFound();
            }
            return Ok(region);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la región por ID");
            return StatusCode(500, "Error al obtener la región por ID");
        }
    }
}
