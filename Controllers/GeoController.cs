using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con la información geográfica.
/// Proporciona endpoints para la gestión de ciudades y regiones,
/// incluyendo búsquedas por ID y listados completos.
/// </summary>
[Route("geo")]
[ApiController]
public class GeoController(ILogger<GeoController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<GeoController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene una ciudad por su ID
    /// </summary>
    /// <param name="queryParameters">Los parámetros de la consulta</param>
    /// <returns>La ciudad encontrada</returns>
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

            var city = await _unitOfWork.GeoRepository.GetCityById(queryParameters.CityId);

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

    /// <summary>
    /// Obtiene una región por su ID
    /// </summary>
    /// <param name="queryParameters">Los parámetros de la consulta</param>
    /// <returns>La región encontrada</returns>
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

            var region = await _unitOfWork.GeoRepository.GetRegionById(queryParameters.RegionId);

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

    /// <summary>
    /// Obtiene todas las ciudades de la base de datos
    /// </summary>
    /// <param name="queryParameters">Los parámetros de la consulta</param>
    /// <returns>Las ciudades encontradas</returns>
    [HttpGet("get-all-cities-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las ciudades de la base de datos", Description = "Devuelve una lista de todas las ciudades.")]
    public async Task<IActionResult> GetCities([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo ciudades de la base de datos");
                var result = await _unitOfWork.GeoRepository.GetAllCitiesFromDb(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las ciudades");
            return StatusCode(500, "Error al obtener las ciudades");
        }
    }

    /// <summary>
    /// Obtiene todas las regiones de la base de datos
    /// </summary>
    /// <param name="queryParameters">Los parámetros de la consulta</param>
    /// <returns>Las regiones encontradas</returns>
    [HttpGet("get-all-regions-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las regiones de la base de datos", Description = "Devuelve una lista de todas las regiones.")]
    public async Task<IActionResult> GetAllRegions([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo regiones de la base de datos");
                var result = await _unitOfWork.GeoRepository.GetAllRegionsFromDb(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las regiones");
            return StatusCode(500, "Error al obtener las regiones");
        }
    }

    /// <summary>
    /// Obtiene todas las regiones por ID de ciudad
    /// </summary>
    /// <param name="queryParameters">Los parámetros de la consulta</param>
    /// <returns>Las regiones encontradas</returns>
    [HttpGet("get-regions-by-city-id")]
    [SwaggerOperation(Summary = "Obtiene todas las regiones por ID de ciudad", Description = "Devuelve una lista de todas las regiones por ID de ciudad.")]
    public async Task<IActionResult> GetRegionsByCityId([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo regiones por ID de ciudad: {Id}", queryParameters.CityId);

                if (queryParameters.CityId == 0)
                {
                    return BadRequest("El ID de la ciudad es requerido");
                }

                var result = await _unitOfWork.GeoRepository.GetRegionsByCityId(queryParameters.CityId, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las regiones por ID de ciudad");
            return StatusCode(500, "Error al obtener las regiones por ID de ciudad");
        }
    }

    /// <summary>
    /// Obtiene todas las ciudades por ID de región
    /// </summary>
    /// <param name="queryParameters">Los parámetros de la consulta</param>
    /// <returns>Las ciudades encontradas</returns> 
    [HttpGet("get-cities-by-region-id")]
    [SwaggerOperation(Summary = "Obtiene todas las ciudades por ID de región", Description = "Devuelve una lista de todas las ciudades por ID de región.")]
    public async Task<IActionResult> GetCitiesByRegionId([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo ciudades por ID de región: {Id}", queryParameters.RegionId);

                if (queryParameters.RegionId == 0)
                {
                    return BadRequest("El ID de la región es requerido");
                }

                var result = await _unitOfWork.GeoRepository.GetCitiesByRegionId(queryParameters.RegionId, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las ciudades por ID de región");
            return StatusCode(500, "Error al obtener las ciudades por ID de región");
        }
    }
}

