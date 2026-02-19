using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Api.Models;
using Api.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Hubs;

public interface IMessagesClient
{
    Task MessageCreated(Message message);
    Task MessageUpdated(Message message);
    Task MessageDeleted(int id);
    Task MessageRead(int id);
    Task UnreadCountChanged(int count);
}

// Quitando autorización para permitir SignalR sin autenticación
public class MessageHub : Hub<IMessagesClient>
{
    private readonly ILogger<MessageHub> _logger;

    public MessageHub(ILogger<MessageHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("[SignalR] Usuario conectado - ConnectionId: {ConnectionId}", Context.ConnectionId);
        _logger.LogInformation("[SignalR] User autenticado: {IsAuthenticated}, Claims Count: {ClaimsCount}",
            Context.User?.Identity?.IsAuthenticated, Context.User?.Claims?.Count() ?? 0);

        string? userId = null;

        // Intentar obtener userId del usuario autenticado
        if (Context.User?.Identity?.IsAuthenticated == true)
        {
            userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("nameid")?.Value
                     ?? Context.User?.FindFirst("sub")?.Value;

            if (Context.User?.Claims != null)
            {
                foreach (var claim in Context.User.Claims)
                {
                    _logger.LogDebug("[SignalR] Claim: {ClaimType} = {ClaimValue}", claim.Type, claim.Value);
                }
            }
        }
        else
        {
            // Si no está autenticado, intentar obtener el token de la query string y decodificarlo manualmente
            _logger.LogInformation("[SignalR] Usuario NO autenticado, intentando decodificar token manualmente");
            var httpContext = Context.GetHttpContext();
            var token = httpContext?.Request.Query["access_token"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("[SignalR] Token encontrado en query string, decodificando...");
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(token);

                    _logger.LogDebug("[SignalR] Token decodificado - Claims Count: {ClaimsCount}", jsonToken.Claims.Count());

                    // Buscar userId en los claims del token
                    userId = jsonToken.Claims.FirstOrDefault(c =>
                        c.Type == ClaimTypes.NameIdentifier ||
                        c.Type == "nameid" ||
                        c.Type == "sub" ||
                        c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                    )?.Value;

                    if (!string.IsNullOrEmpty(userId))
                    {
                        _logger.LogInformation("[SignalR] UserId obtenido manualmente del token: {UserId}", userId);

                        foreach (var claim in jsonToken.Claims)
                        {
                            _logger.LogDebug("[SignalR] Token Claim: {ClaimType} = {ClaimValue}", claim.Type, claim.Value);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("[SignalR] No se encontró userId en los claims del token");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[SignalR] Error decodificando token manualmente");
                }
            }
            else
            {
                _logger.LogWarning("[SignalR] No se encontró token en query string");
            }
        }

        _logger.LogInformation("[SignalR] UserId final encontrado: {UserId}", userId ?? "NULL");

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            _logger.LogInformation("[SignalR] Usuario agregado al grupo: user:{UserId}", userId);
        }
        else
        {
            _logger.LogWarning("[SignalR] No se pudo obtener userId del token - usuario solo en grupo broadcast");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, "broadcast");
        _logger.LogInformation("[SignalR] Usuario agregado al grupo: broadcast");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("[SignalR] Usuario desconectado - ConnectionId: {ConnectionId}", Context.ConnectionId);
        if (exception != null)
        {
            _logger.LogError(exception, "[SignalR] Error en desconexión");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
