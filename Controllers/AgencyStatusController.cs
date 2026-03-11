using Api.Interfaces;
using Api.Filters;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controller that manages all operations related to agency statuses.
/// Provides endpoints for full CRUD management of agency statuses.
/// </summary>
[ApiController]
[Route("agency-status")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class AgencyStatusController(IAgencyStatusRepository agencyStatusRepository) : ControllerBase
{
    private readonly IAgencyStatusRepository _agencyStatusRepository = agencyStatusRepository;

    /// <summary>
    /// Gets an agency status by its ID.
    /// </summary>
    /// <param name="queryParameters">Query parameters containing the ID.</param>
    /// <returns>The agency status if found, NotFound if not, or InternalServerError on error.</returns>
    [HttpGet("get-agency-status-by-id")]
    [SwaggerOperation(Summary = "Gets an agency status by its ID", Description = "Returns an agency status based on the provided ID.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("The agency status ID is required");
        }

        var result = await _agencyStatusRepository.GetAgencyStatusById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Agency status with ID {queryParameters.Id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets all agency statuses with filtering and pagination options.
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering and pagination.</param>
    /// <returns>List of agency statuses, BadRequest if parameters are invalid, or InternalServerError on error.</returns>
    [HttpGet("get-all-agency-status-from-db")]
    [SwaggerOperation(Summary = "Gets all agency statuses", Description = "Returns a list of agency statuses.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _agencyStatusRepository.GetAllAgencyStatuses(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

        if (result == null)
        {
            return NotFound("No agency statuses found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new agency status.
    /// </summary>
    /// <param name="request">The agency status to create.</param>
    /// <returns>Ok if created successfully, BadRequest if data is invalid or creation fails, or InternalServerError on error.</returns>
    [HttpPost("insert-agency-status")]
    [SwaggerOperation(Summary = "Creates a new agency status", Description = "Creates a new agency status.")]
    public async Task<ActionResult> Insert([FromBody] AgencyStatusRequest request)
    {
        var result = await _agencyStatusRepository.InsertAgencyStatus(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("Could not create the agency status");
    }

    /// <summary>
    /// Updates an existing agency status.
    /// </summary>
    /// <param name="request">The agency status with updated data.</param>
    /// <returns>NoContent if updated successfully, NotFound if not found, BadRequest if data is invalid, or InternalServerError on error.</returns>
    [HttpPut("update-agency-status")]
    [SwaggerOperation(Summary = "Updates an existing agency status", Description = "Updates the data of an existing agency status.")]
    public async Task<IActionResult> Update([FromBody] DTOAgencyStatus request)
    {
        var result = await _agencyStatusRepository.UpdateAgencyStatus(request);

        if (!result)
        {
            return NotFound($"Agency status with ID {request.Id} not found");
        }

        return NoContent();
    }

    /// <summary>
    /// Updates the display order of an agency status.
    /// </summary>
    /// <param name="statusId">ID of the agency status to update.</param>
    /// <param name="displayOrder">New display order.</param>
    /// <returns>NoContent if updated successfully, NotFound if not found, BadRequest if data is invalid, or InternalServerError on error.</returns>
    [HttpPut("update-agency-status-display-order")]
    [SwaggerOperation(Summary = "Updates the display order of an agency status", Description = "Updates the display order of an existing agency status.")]
    public async Task<IActionResult> UpdateDisplayOrder([FromQuery] int statusId, [FromQuery] int displayOrder)
    {
        var result = await _agencyStatusRepository.UpdateAgencyStatusDisplayOrder(statusId, displayOrder);

        if (!result)
        {
            return NotFound($"Agency status with ID {statusId} not found");
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes an existing agency status.
    /// </summary>
    /// <param name="queryParameters">Query parameters containing the ID.</param>
    /// <returns>NoContent if deleted successfully, NotFound if not found, BadRequest if data is invalid, or InternalServerError on error.</returns>
    [HttpDelete("delete-agency-status")]
    [SwaggerOperation(Summary = "Deletes an existing agency status", Description = "Deletes an existing agency status.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        var result = await _agencyStatusRepository.DeleteAgencyStatus(queryParameters.Id);

        if (!result)
        {
            return NotFound($"Agency status with ID {queryParameters.Id} not found");
        }

        return NoContent();
    }
}
