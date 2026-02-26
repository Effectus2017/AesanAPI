using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controller that manages all operations related to agency status history.
/// Provides endpoints for viewing the history of status changes for agencies.
/// </summary>
[ApiController]
[Route("agency-status-history")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AgencyStatusHistoryController(IAgencyStatusHistoryRepository agencyStatusHistoryRepository, ILogger<AgencyStatusHistoryController> logger) : ControllerBase
{
    private readonly IAgencyStatusHistoryRepository _agencyStatusHistoryRepository = agencyStatusHistoryRepository;
    private readonly ILogger<AgencyStatusHistoryController> _logger = logger;

    /// <summary>
    /// Gets the agency status history with pagination and optional filters.
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering and pagination.</param>
    /// <returns>List of agency status history records, BadRequest if parameters are invalid, or InternalServerError on error.</returns>
    [HttpGet("get-agency-status-history-paged")]
    [SwaggerOperation(Summary = "Gets agency status history", Description = "Returns the history of status changes for agencies with pagination and optional filters.")]
    public async Task<ActionResult> GetAgencyStatusHistoryPaged([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Getting agency status history");

                var result = await _agencyStatusHistoryRepository.GetAgencyStatusHistoryPaged(
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.AgencyId > 0 ? queryParameters.AgencyId : null,
                    queryParameters.CreatedAtFrom,
                    queryParameters.CreatedAtTo);

                if (result == null)
                {
                    return NotFound("No agency status history found");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agency status history");
            return StatusCode(500, "Internal server error while getting agency status history");
        }
    }
}
