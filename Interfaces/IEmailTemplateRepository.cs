using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

public interface IEmailTemplateRepository
{
    /// <summary>
    /// Obtiene un template de email por su ID
    /// </summary>
    /// <param name="id">El ID del template</param>
    /// <returns>El template encontrado o null si no se encuentra</returns>
    Task<EmailTemplateResponse?> GetEmailTemplateById(int id);

    /// <summary>
    /// Obtiene un template de email por su clave
    /// </summary>
    /// <param name="templateKey">La clave del template</param>
    /// <returns>El template encontrado o null si no se encuentra</returns>
    Task<EmailTemplateResponse?> GetEmailTemplateByKey(string templateKey);

    /// <summary>
    /// Obtiene todos los templates de email
    /// </summary>
    /// <param name="take">El número de templates a tomar</param>
    /// <param name="skip">El número de templates a saltar</param>
    /// <param name="templateKey">Filtro por clave de template</param>
    /// <param name="description">Filtro por descripción</param>
    /// <param name="alls">Si se deben obtener todos los templates (incluyendo inactivos)</param>
    /// <returns>Lista de templates y total count</returns>
    Task<dynamic> GetAllEmailTemplates(int take, int skip, string? templateKey, string? description, bool alls);

    /// <summary>
    /// Inserta un nuevo template de email
    /// </summary>
    /// <param name="emailTemplate">El template a insertar</param>
    /// <param name="createdBy">ID del usuario que crea el template</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario</returns>
    Task<bool> InsertEmailTemplate(EmailTemplateRequest emailTemplate, string? createdBy = null);

    /// <summary>
    /// Actualiza un template de email existente
    /// </summary>
    /// <param name="emailTemplate">El template a actualizar</param>
    /// <param name="updatedBy">ID del usuario que actualiza el template</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario</returns>
    Task<bool> UpdateEmailTemplate(EmailTemplateRequest emailTemplate, string? updatedBy = null);
}

