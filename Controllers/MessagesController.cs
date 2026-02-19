using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Models.Request;
using Dapper;
using System.Security.Claims;
using Api.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar mensajes
/// Proporciona endpoints para la gestión completa de mensajes, incluyendo creación,
/// lectura, actualización y eliminación de mensajes.
/// </summary>
[ApiController]
[Route("messages")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MessagesController(ILogger<MessagesController> logger, IUnitOfWork unitOfWork, IHubContext<MessageHub, IMessagesClient> hub) : Controller
{
    private readonly ILogger<MessagesController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly IHubContext<MessageHub, IMessagesClient> _hub = hub;

    /// <summary>
    /// Obtiene un mensaje por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El mensaje si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-message-by-id")]
    [SwaggerOperation(Summary = "Obtiene un mensaje por su ID", Description = "Devuelve un mensaje basado en el ID proporcionado.")]
    public async Task<IActionResult> GetMessageById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo mensaje por ID: {Id}", queryParameters.Id);

                var message = await _unitOfWork.MessageRepository.GetMessageById(queryParameters.Id);

                if (message == null)
                {
                    return NotFound($"Mensaje con ID {queryParameters.Id} no encontrado");
                }

                return Ok(message);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el mensaje con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el mensaje");
        }
    }

    /// <summary>
    /// Obtiene todos los mensajes de la base de datos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>La lista de mensajes si se encuentran, NotFound si no se encuentran, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-messages")]
    [SwaggerOperation(Summary = "Obtiene todos los mensajes de la base de datos", Description = "Devuelve una lista de todos los mensajes.")]
    public async Task<IActionResult> GetAllMessages([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo todos los mensajes");

                var messages = await _unitOfWork.MessageRepository.GetAllMessagesFromDb(queryParameters.UserId);
                return Ok(messages);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los mensajes");
            return StatusCode(500, "Error al obtener los mensajes");
        }
    }

    /// <summary>
    /// Inserta un nuevo mensaje
    /// </summary>
    /// <param name="request">El mensaje a insertar</param>
    /// <returns>El resultado de la inserción</returns>
    [HttpPost("insert-message")]
    [SwaggerOperation(Summary = "Inserta un nuevo mensaje", Description = "Crea un nuevo mensaje en la base de datos.")]
    public async Task<IActionResult> InsertMessage([FromBody] MessageRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("El mensaje es requerido");
                }

                var created = await _unitOfWork.MessageRepository.InsertMessage(request);

                if (!string.IsNullOrEmpty(created.UserId))
                {
                    var groupName = $"user:{created.UserId}";
                    _logger.LogInformation("[MessagesController] Enviando mensaje al grupo {GroupName}, MessageId: {MessageId}", groupName, created.Id);

                    try
                    {
                        await _hub.Clients.Group(groupName).MessageCreated(created);
                        var unread = await _unitOfWork.MessageRepository.GetUnreadMessageCount(created.UserId);
                        await _hub.Clients.Group(groupName).UnreadCountChanged(unread);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[MessagesController] Error al enviar mensaje via SignalR al grupo {GroupName}", groupName);
                    }
                }
                else
                {
                    await _hub.Clients.Group("broadcast").MessageCreated(created);
                }

                _logger.LogInformation("Mensaje insertado correctamente");
                return Ok(created);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el mensaje");
            return StatusCode(500, "Error al insertar el mensaje");
        }
    }

    /// <summary>
    /// Actualiza un mensaje existente
    /// </summary>
    /// <param name="request">El mensaje a actualizar</param>
    /// <returns>El resultado de la actualización</returns>
    [HttpPut("update-message")]
    [SwaggerOperation(Summary = "Actualiza un mensaje existente", Description = "Actualiza un mensaje existente en la base de datos.")]
    public async Task<IActionResult> UpdateMessage([FromBody] MessageRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("El mensaje es requerido");
                }

                if (!request.Id.HasValue)
                {
                    return BadRequest("El ID del mensaje es requerido para actualizar");
                }

                var success = await _unitOfWork.MessageRepository.UpdateMessage(request);
                if (!success) return BadRequest("No se pudo actualizar el mensaje");

                if (request.Id is int id)
                {
                    var updated = await _unitOfWork.MessageRepository.GetMessageById(id);
                    if (updated is Message msg)
                    {
                        if (!string.IsNullOrEmpty(msg.UserId))
                        {
                            var groupName = $"user:{msg.UserId}";
                            await _hub.Clients.Group(groupName).MessageUpdated(msg);
                        }
                        else
                        {
                            await _hub.Clients.Group("broadcast").MessageUpdated(msg);
                        }
                    }
                }

                _logger.LogInformation("Mensaje actualizado con ID: {Id}", request.Id.Value);
                return Ok(true);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el mensaje");
            return StatusCode(500, "Error al actualizar el mensaje");
        }
    }

    /// <summary>
    /// Elimina un mensaje (baja lógica)
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El resultado de la eliminación</returns>
    [HttpDelete("delete-message")]
    [SwaggerOperation(Summary = "Elimina un mensaje", Description = "Realiza una baja lógica del mensaje en la base de datos.")]
    public async Task<IActionResult> DeleteMessage([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Eliminando mensaje con ID: {Id}", queryParameters.Id);

                var id = queryParameters.Id;
                var existing = await _unitOfWork.MessageRepository.GetMessageById(id);
                var success = await _unitOfWork.MessageRepository.DeleteMessage(id);
                if (!success) return BadRequest("No se pudo eliminar el mensaje");

                if (existing is Message msg)
                {
                    if (!string.IsNullOrEmpty(msg.UserId))
                    {
                        var groupName = $"user:{msg.UserId}";
                        await _hub.Clients.Group(groupName).MessageDeleted(msg.Id);
                    }
                    else
                    {
                        await _hub.Clients.Group("broadcast").MessageDeleted(msg.Id);
                    }
                }

                _logger.LogInformation("Mensaje eliminado con ID: {Id}", queryParameters.Id);
                return Ok(true);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el mensaje con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error al eliminar el mensaje");
        }
    }

    /// <summary>
    /// Marca todos los mensajes como leídos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>El resultado de la operación</returns>
    [HttpPost("mark-all-as-read")]
    [SwaggerOperation(Summary = "Marca todos los mensajes como leídos", Description = "Marca todos los mensajes del usuario como leídos.")]
    public async Task<IActionResult> MarkAllMessagesAsRead([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Marcando todos los mensajes como leídos para usuario: {UserId}", queryParameters.UserId);

                var userId = queryParameters.UserId;
                var success = await _unitOfWork.MessageRepository.MarkAllMessagesAsRead(userId);
                if (!success) return BadRequest("No se pudieron marcar todos los mensajes como leídos");

                if (!string.IsNullOrEmpty(userId))
                {
                    var groupName = $"user:{userId}";
                    await _hub.Clients.Group(groupName).UnreadCountChanged(0);
                }

                _logger.LogInformation("Todos los mensajes marcados como leídos para usuario: {UserId}", queryParameters.UserId);
                return Ok(new { message = "Todos los mensajes han sido marcados como leídos" });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar todos los mensajes como leídos");
            return StatusCode(500, "Error al marcar todos los mensajes como leídos");
        }
    }

    /// <summary>
    /// Marca un mensaje específico como leído
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El resultado de la operación</returns>
    [HttpPost("mark-message-as-read")]
    [SwaggerOperation(Summary = "Marca un mensaje como leído", Description = "Marca un mensaje específico como leído.")]
    public async Task<IActionResult> MarkMessageAsRead([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Marcando mensaje como leído con ID: {Id}", queryParameters.Id);

                var id = queryParameters.Id;
                var existing = await _unitOfWork.MessageRepository.GetMessageById(id);
                var success = await _unitOfWork.MessageRepository.MarkMessageAsRead(id);
                if (!success) return BadRequest("No se pudo marcar como leído");

                if (existing is Message msg)
                {
                    if (!string.IsNullOrEmpty(msg.UserId))
                    {
                        var groupName = $"user:{msg.UserId}";
                        await _hub.Clients.Group(groupName).MessageRead(msg.Id);
                        var unread = await _unitOfWork.MessageRepository.GetUnreadMessageCount(msg.UserId);
                        await _hub.Clients.Group(groupName).UnreadCountChanged(unread);
                    }
                    else
                    {
                        await _hub.Clients.Group("broadcast").MessageRead(msg.Id);
                    }
                }

                _logger.LogInformation("Mensaje marcado como leído con ID: {Id}", queryParameters.Id);
                return Ok(new { message = "Mensaje marcado como leído" });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar el mensaje como leído con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error al marcar el mensaje como leído");
        }
    }

    /// <summary>
    /// Obtiene el conteo de mensajes no leídos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>El número de mensajes no leídos</returns>
    [HttpGet("get-unread-message-count")]
    [SwaggerOperation(Summary = "Obtiene el conteo de mensajes no leídos", Description = "Devuelve el número de mensajes no leídos del usuario.")]
    public async Task<IActionResult> GetUnreadMessageCount([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo conteo de mensajes no leídos para usuario: {UserId}", queryParameters.UserId);

                var count = await _unitOfWork.MessageRepository.GetUnreadMessageCount(queryParameters.UserId);
                return Ok(count);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el conteo de mensajes no leídos");
            return StatusCode(500, "Error al obtener el conteo de mensajes no leídos");
        }
    }
}