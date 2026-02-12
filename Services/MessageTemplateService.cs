using Api.Interfaces;
using Api.Models.Request;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;
using Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Services;

/// <summary>
/// Service for sending messages and emails using templates
/// Servicio para enviar mensajes y emails usando templates
/// </summary>
public class MessageTemplateService
{
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IEmailTemplateRepository _emailTemplateRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IEmailService _emailService;
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageTemplateService> _logger;

    public MessageTemplateService(
        IMessageTemplateRepository messageTemplateRepository,
        IEmailTemplateRepository emailTemplateRepository,
        IMessageRepository messageRepository,
        IEmailService emailService,
        IHubContext<MessageHub> hubContext,
        IServiceProvider serviceProvider,
        ILogger<MessageTemplateService> logger)
    {
        _messageTemplateRepository = messageTemplateRepository;
        _emailTemplateRepository = emailTemplateRepository;
        _messageRepository = messageRepository;
        _emailService = emailService;
        _hubContext = hubContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Envía mensaje interno Y email usando templates separados
    /// </summary>
    /// <param name="messageTemplateKey">Clave del template de mensaje interno</param>
    /// <param name="emailTemplateKey">Clave del template de email</param>
    /// <param name="recipientUserId">ID del usuario destinatario</param>
    /// <param name="variables">Variables para reemplazar en los templates</param>
    /// <param name="language">Idioma ('es' o 'en'), default 'es'</param>
    public async Task SendMessageAndEmailFromTemplate(
        string messageTemplateKey,
        string emailTemplateKey,
        string recipientUserId,
        Dictionary<string, string> variables,
        string language = "es")
    {
        try
        {
            SignalRLogger.LogToFile($"[MessageTemplateService] SendMessageAndEmailFromTemplate - INICIO - MessageTemplateKey: {messageTemplateKey}, EmailTemplateKey: {emailTemplateKey}, RecipientUserId: {recipientUserId}");
            _logger.LogInformation("Iniciando envío de mensaje y email usando templates. MessageTemplateKey: {MessageTemplateKey}, EmailTemplateKey: {EmailTemplateKey}, RecipientUserId: {RecipientUserId}",
                messageTemplateKey, emailTemplateKey, recipientUserId);

            // 1. Obtener MessageTemplate
            SignalRLogger.LogToFile($"[MessageTemplateService] Buscando MessageTemplate con key: {messageTemplateKey}");
            var messageTemplate = await _messageTemplateRepository.GetMessageTemplateByKey(messageTemplateKey);
            if (messageTemplate == null)
            {
                SignalRLogger.LogToFile($"[MessageTemplateService] ERROR: MessageTemplate {messageTemplateKey} no encontrado en la base de datos");
                _logger.LogError("MessageTemplate {TemplateKey} no encontrado en la base de datos. Verificar que el template existe.", messageTemplateKey);
                return;
            }

            SignalRLogger.LogToFile($"[MessageTemplateService] MessageTemplate encontrado: {messageTemplateKey}, TitleES: {messageTemplate.TitleES}");
            _logger.LogInformation("MessageTemplate encontrado: {TemplateKey}, TitleES: {TitleES}", messageTemplateKey, messageTemplate.TitleES);

            // 2. Obtener EmailTemplate
            SignalRLogger.LogToFile($"[MessageTemplateService] Buscando EmailTemplate con key: {emailTemplateKey}");
            var emailTemplate = await _emailTemplateRepository.GetEmailTemplateByKey(emailTemplateKey);
            if (emailTemplate == null)
            {
                SignalRLogger.LogToFile($"[MessageTemplateService] WARNING: EmailTemplate {emailTemplateKey} no encontrado, continuando solo con mensaje interno");
                _logger.LogWarning("EmailTemplate {TemplateKey} no encontrado, continuando solo con mensaje interno", emailTemplateKey);
                // Continuar solo con mensaje interno si no hay email template
            }
            else
            {
                SignalRLogger.LogToFile($"[MessageTemplateService] EmailTemplate encontrado: {emailTemplateKey}, SubjectES: {emailTemplate.SubjectES}");
                _logger.LogInformation("EmailTemplate encontrado: {TemplateKey}, SubjectES: {SubjectES}", emailTemplateKey, emailTemplate.SubjectES);
            }

            // 3. Obtener email del usuario destinatario (usando inyección lazy para evitar dependencia circular)
            SignalRLogger.LogToFile($"[MessageTemplateService] Buscando usuario con UserId: {recipientUserId}");
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var user = await userRepository.GetUserByIdWithSP(recipientUserId);
            if (user == null)
            {
                SignalRLogger.LogToFile($"[MessageTemplateService] ERROR: Usuario {recipientUserId} no encontrado en la base de datos");
                _logger.LogError("Usuario {UserId} no encontrado en la base de datos. No se puede enviar el mensaje.", recipientUserId);
                return;
            }

            SignalRLogger.LogToFile($"[MessageTemplateService] Usuario encontrado: {recipientUserId}, Email: {user.Email}");
            _logger.LogInformation("Usuario encontrado: {UserId}, Email: {Email}", recipientUserId, user.Email);

            // 4. Procesar MessageTemplate con variables
            string messageTitle = language.ToLower() == "en" ? messageTemplate.TitleEN : messageTemplate.TitleES;
            string messageBody = language.ToLower() == "en" ? messageTemplate.BodyEN : messageTemplate.BodyES;

            foreach (var variable in variables)
            {
                messageTitle = messageTitle.Replace($"{{{variable.Key}}}", variable.Value);
                messageBody = messageBody.Replace($"{{{variable.Key}}}", variable.Value);
            }

            // 5. Enviar mensaje interno
            var message = new MessageRequest
            {
                Title = messageTitle,
                Description = messageBody,
                Icon = messageTemplate.Icon,
                Image = messageTemplate.Image,
                Link = messageTemplate.Link,
                UseRouter = messageTemplate.UseRouter,
                UserId = recipientUserId
            };

            SignalRLogger.LogToFile($"[MessageTemplateService] Insertando mensaje en BD. Title: {messageTitle}, UserId: {recipientUserId}");
            _logger.LogInformation("Insertando mensaje en la base de datos. Title: {Title}, UserId: {UserId}", messageTitle, recipientUserId);
            var createdMessage = await _messageRepository.InsertMessage(message);

            if (createdMessage == null)
            {
                SignalRLogger.LogToFile("[MessageTemplateService] ERROR: El mensaje no se creó (createdMessage es null)");
                _logger.LogError("Error al insertar mensaje en la base de datos. El mensaje no se creó.");
                return;
            }

            SignalRLogger.LogToFile($"[MessageTemplateService] Mensaje insertado exitosamente. MessageId: {createdMessage.Id}");
            _logger.LogInformation("Mensaje insertado exitosamente. MessageId: {MessageId}", createdMessage.Id);

            // Enviar via SignalR
            var groupName = $"user:{recipientUserId}";
            SignalRLogger.LogToFile($"[MessageTemplateService] Enviando mensaje via SignalR al grupo: {groupName}");
            _logger.LogInformation("Enviando mensaje via SignalR al grupo: {GroupName}", groupName);
            await _hubContext.Clients.Group(groupName).SendAsync("MessageCreated", createdMessage);
            SignalRLogger.LogToFile($"[MessageTemplateService] Mensaje enviado via SignalR exitosamente. MessageId: {createdMessage.Id}");
            _logger.LogInformation("Mensaje interno enviado a usuario {UserId} usando template {TemplateKey}. MessageId: {MessageId}",
                recipientUserId, messageTemplateKey, createdMessage.Id);

            // 6. Enviar email si existe template
            if (emailTemplate != null)
            {
                SignalRLogger.LogToFile($"[MessageTemplateService] Preparando envío de email. UserEmail: {user.Email}");
                string emailSubject = language.ToLower() == "en" ? emailTemplate.SubjectEN : emailTemplate.SubjectES;
                string emailBody = language.ToLower() == "en" ? emailTemplate.BodyEN : emailTemplate.BodyES;

                foreach (var variable in variables)
                {
                    emailSubject = emailSubject.Replace($"{{{variable.Key}}}", variable.Value);
                    emailBody = emailBody.Replace($"{{{variable.Key}}}", variable.Value);
                }

                try
                {
                    SignalRLogger.LogToFile($"[MessageTemplateService] Llamando a SendEmailWithGmail. Email: {user.Email}, Subject: {emailSubject}");
                    await _emailService.SendEmailWithGmail(user.Email, emailSubject, emailBody);
                    SignalRLogger.LogToFile($"[MessageTemplateService] Email enviado exitosamente a {user.Email}");
                    _logger.LogInformation("Email enviado a {Email} usando template {TemplateKey}", user.Email, emailTemplateKey);
                }
                catch (Exception emailEx)
                {
                    SignalRLogger.LogToFile($"[MessageTemplateService] ERROR al enviar email: {emailEx.Message}");
                    SignalRLogger.LogToFile($"[MessageTemplateService] StackTrace del error de email: {emailEx.StackTrace}");
                    _logger.LogError(emailEx, "Error al enviar email a {Email} usando template {TemplateKey}", user.Email, emailTemplateKey);
                    // No lanzar excepción para no interrumpir el flujo del mensaje interno
                }
            }
            else
            {
                SignalRLogger.LogToFile($"[MessageTemplateService] EmailTemplate es null, no se enviará email");
            }
        }
        catch (Exception ex)
        {
            SignalRLogger.LogToFile($"[MessageTemplateService] EXCEPCIÓN: {ex.Message}");
            SignalRLogger.LogToFile($"[MessageTemplateService] StackTrace: {ex.StackTrace}");
            _logger.LogError(ex, "Error al enviar mensaje y email desde templates");
            throw;
        }
    }
}

