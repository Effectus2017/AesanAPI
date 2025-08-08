using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

public interface IMessageRepository
{
    /// <summary>
    /// Obtiene un mensaje por su ID
    /// </summary>
    /// <param name="id">El ID del mensaje</param>
    /// <returns>El mensaje si se encuentra</returns>
    Task<dynamic> GetMessageById(int id);

    /// <summary>
    /// Obtiene todos los mensajes de la base de datos
    /// </summary>
    /// <param name="userId">ID del usuario (opcional)</param>
    /// <returns>Lista de mensajes</returns>
    Task<dynamic> GetAllMessagesFromDb(string? userId = null);

    /// <summary>
    /// Inserta un nuevo mensaje en la base de datos
    /// </summary>
    /// <param name="messageRequest">Datos del mensaje a insertar</param>
    /// <returns>True si se insertó correctamente</returns>
    Task<bool> InsertMessage(MessageRequest messageRequest);

    /// <summary>
    /// Actualiza un mensaje existente en la base de datos
    /// </summary>
    /// <param name="messageRequest">Datos del mensaje a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateMessage(MessageRequest messageRequest);

    /// <summary>
    /// Elimina un mensaje de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID del mensaje a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> DeleteMessage(int id);

    /// <summary>
    /// Marca todos los mensajes como leídos
    /// </summary>
    /// <param name="userId">ID del usuario (opcional)</param>
    /// <returns>True si se marcaron correctamente</returns>
    Task<bool> MarkAllMessagesAsRead(string? userId = null);

    /// <summary>
    /// Marca un mensaje específico como leído
    /// </summary>
    /// <param name="id">El ID del mensaje</param>
    /// <returns>True si se marcó correctamente</returns>
    Task<bool> MarkMessageAsRead(int id);

    /// <summary>
    /// Obtiene el conteo de mensajes no leídos
    /// </summary>
    /// <param name="userId">ID del usuario (opcional)</param>
    /// <returns>Número de mensajes no leídos</returns>
    Task<int> GetUnreadMessageCount(string? userId = null);
}