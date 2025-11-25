using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

public interface IMessageTemplateRepository
{
    /// <summary>
    /// Obtiene un template de mensaje por su ID
    /// </summary>
    /// <param name="id">El ID del template</param>
    /// <returns>El template encontrado o null si no se encuentra</returns>
    Task<MessageTemplateResponse?> GetMessageTemplateById(int id);

    /// <summary>
    /// Obtiene un template de mensaje por su clave
    /// </summary>
    /// <param name="templateKey">La clave del template</param>
    /// <returns>El template encontrado o null si no se encuentra</returns>
    Task<MessageTemplateResponse?> GetMessageTemplateByKey(string templateKey);

    /// <summary>
    /// Obtiene todos los templates de mensaje
    /// </summary>
    /// <param name="take">El número de templates a tomar</param>
    /// <param name="skip">El número de templates a saltar</param>
    /// <param name="templateKey">Filtro por clave de template</param>
    /// <param name="description">Filtro por descripción</param>
    /// <param name="alls">Si se deben obtener todos los templates (incluyendo inactivos)</param>
    /// <returns>Lista de templates y total count</returns>
    Task<dynamic> GetAllMessageTemplates(int take, int skip, string? templateKey, string? description, bool alls);

    /// <summary>
    /// Inserta un nuevo template de mensaje
    /// </summary>
    /// <param name="messageTemplate">El template a insertar</param>
    /// <param name="createdBy">ID del usuario que crea el template</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario</returns>
    Task<bool> InsertMessageTemplate(MessageTemplateRequest messageTemplate, string? createdBy = null);

    /// <summary>
    /// Actualiza un template de mensaje existente
    /// </summary>
    /// <param name="messageTemplate">El template a actualizar</param>
    /// <param name="updatedBy">ID del usuario que actualiza el template</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario</returns>
    Task<bool> UpdateMessageTemplate(MessageTemplateRequest messageTemplate, string? updatedBy = null);
}

