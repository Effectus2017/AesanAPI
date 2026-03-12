using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
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
[ValidateModelState]
public class MessagesController(IUnitOfWork unitOfWork, IHubContext<MessageHub, IMessagesClient> hub) : Controller
{
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
        var message = await _unitOfWork.MessageRepository.GetMessageById(queryParameters.Id);

        if (message == null)
        {
            return NotFound($"Mensaje con ID {queryParameters.Id} no encontrado");
        }

        return Ok(message);
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
        var messages = await _unitOfWork.MessageRepository.GetAllMessagesFromDb(queryParameters.UserId);
        return Ok(messages);
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
        if (request == null)
        {
            return BadRequest("El mensaje es requerido");
        }

        var created = await _unitOfWork.MessageRepository.InsertMessage(request);

        if (!string.IsNullOrEmpty(created.UserId))
        {
            var groupName = $"user:{created.UserId}";
            await _hub.Clients.Group(groupName).MessageCreated(created);
            var unread = await _unitOfWork.MessageRepository.GetUnreadMessageCount(created.UserId);
            await _hub.Clients.Group(groupName).UnreadCountChanged(unread);
        }
        else
        {
            await _hub.Clients.Group("broadcast").MessageCreated(created);
        }

        return Ok(created);
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
        if (request == null)
        {
            return BadRequest("El mensaje es requerido");
        }

        if (!request.Id.HasValue)
        {
            return BadRequest("El ID del mensaje es requerido para actualizar");
        }

        var success = await _unitOfWork.MessageRepository.UpdateMessage(request);
        if (!success)
        {
            return BadRequest("No se pudo actualizar el mensaje");
        }

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

        return Ok(true);
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
        var id = queryParameters.Id;
        var existing = await _unitOfWork.MessageRepository.GetMessageById(id);
        var success = await _unitOfWork.MessageRepository.DeleteMessage(id);
        if (!success)
        {
            return BadRequest("No se pudo eliminar el mensaje");
        }

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

        return Ok(true);
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
        var userId = queryParameters.UserId;
        var success = await _unitOfWork.MessageRepository.MarkAllMessagesAsRead(userId);
        if (!success)
        {
            return BadRequest("No se pudieron marcar todos los mensajes como leídos");
        }

        if (!string.IsNullOrEmpty(userId))
        {
            var groupName = $"user:{userId}";
            await _hub.Clients.Group(groupName).UnreadCountChanged(0);
        }

        return Ok(new { message = "Todos los mensajes han sido marcados como leídos" });
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
        var id = queryParameters.Id;
        var existing = await _unitOfWork.MessageRepository.GetMessageById(id);
        var success = await _unitOfWork.MessageRepository.MarkMessageAsRead(id);
        if (!success)
        {
            return BadRequest("No se pudo marcar como leído");
        }

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

        return Ok(new { message = "Mensaje marcado como leído" });
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
        var count = await _unitOfWork.MessageRepository.GetUnreadMessageCount(queryParameters.UserId);
        return Ok(count);
    }
}
