using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controller that manages all operations related to alternative communications.
/// Provides endpoints for full CRUD management of alternative communications.
/// </summary>
[Route("alternative-communication")]
[ApiController]
public class AlternativeCommunicationController(IAlternativeCommunicationRepository alternativeCommunicationRepository, ILogger<AlternativeCommunicationController> logger) : ControllerBase
{
    private readonly IAlternativeCommunicationRepository _alternativeCommunicationRepository = alternativeCommunicationRepository;
    private readonly ILogger<AlternativeCommunicationController> _logger = logger;

    /// <summary>
    /// Gets an alternative communication by its ID.
    /// </summary>
    /// <param name="queryParameters">Query parameters containing the ID.</param>
    /// <returns>The alternative communication if found, NotFound if not, or InternalServerError on error.</returns>
    [HttpGet("get-alternative-communication-by-id")]
    [SwaggerOperation(Summary = "Gets an alternative communication by its ID", Description = "Returns an alternative communication based on the provided ID.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Getting alternative communication by ID: {Id}", queryParameters.Id);

                if (queryParameters.Id == 0)
                {
                    return BadRequest("The alternative communication ID is required");
                }

                var result = await _alternativeCommunicationRepository.GetAlternativeCommunicationById(queryParameters.Id);

                if (result == null)
                {
                    return NotFound($"Alternative communication with ID {queryParameters.Id} not found");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alternative communication by ID {Id}", queryParameters.Id);
            return StatusCode(500, "Internal server error while getting alternative communication");
        }
    }

    /// <summary>
    /// Gets all alternative communications with filtering and pagination options.
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering and pagination.</param>
    /// <returns>List of alternative communications, BadRequest if parameters are invalid, or InternalServerError on error.</returns>
    [HttpGet("get-all-alternative-communications-from-db")]
    [SwaggerOperation(Summary = "Gets all alternative communications", Description = "Returns a list of alternative communications.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _alternativeCommunicationRepository.GetAllAlternativeCommunications(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);

                if (result == null)
                {
                    return NotFound("No alternative communications found");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all alternative communications");
            return StatusCode(500, "Internal server error while getting alternative communications");
        }
    }

    /// <summary>
    /// Creates a new alternative communication.
    /// </summary>
    /// <param name="request">The alternative communication to create.</param>
    /// <returns>Ok if created successfully, BadRequest if data is invalid or creation fails, or InternalServerError on error.</returns>
    [HttpPost("insert-alternative-communication")]
    [SwaggerOperation(Summary = "Creates a new alternative communication", Description = "Creates a new alternative communication.")]
    public async Task<ActionResult> Insert([FromBody] DTOAlternativeCommunication request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _alternativeCommunicationRepository.InsertAlternativeCommunication(request);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("Could not create the alternative communication");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating alternative communication");
            return StatusCode(500, "Internal server error while creating alternative communication");
        }
    }

    /// <summary>
    /// Updates an existing alternative communication.
    /// </summary>
    /// <param name="request">The alternative communication with updated data.</param>
    /// <returns>NoContent if updated successfully, NotFound if not found, BadRequest if data is invalid, or InternalServerError on error.</returns>
    [HttpPut("update-alternative-communication")]
    [SwaggerOperation(Summary = "Updates an existing alternative communication", Description = "Updates the data of an existing alternative communication.")]
    public async Task<IActionResult> Update([FromBody] DTOAlternativeCommunication request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _alternativeCommunicationRepository.UpdateAlternativeCommunication(request);

                if (!result)
                {
                    return NotFound($"Alternative communication with ID {request.Id} not found");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating alternative communication with ID {Id}", request.Id);
            return StatusCode(500, "Internal server error while updating alternative communication");
        }
    }

    /// <summary>
    /// Deletes an existing alternative communication.
    /// </summary>
    /// <param name="id">ID of the alternative communication to delete.</param>
    /// <returns>NoContent if deleted successfully, NotFound if not found, BadRequest if data is invalid, or InternalServerError on error.</returns>
    [HttpDelete("delete-alternative-communication")]
    [SwaggerOperation(Summary = "Deletes an existing alternative communication", Description = "Deletes an existing alternative communication.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _alternativeCommunicationRepository.DeleteAlternativeCommunication(queryParameters.Id);

                if (!result)
                {
                    return NotFound($"Alternative communication with ID {queryParameters.Id} not found");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting alternative communication with ID {Id}", queryParameters.Id);
            return StatusCode(500, "Internal server error while deleting alternative communication");
        }
    }
}