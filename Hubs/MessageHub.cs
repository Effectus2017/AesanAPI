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
    public override async Task OnConnectedAsync()
    {
        SignalRLogger.LogToFile($"[SignalR] Usuario conectado - ConnectionId: {Context.ConnectionId}");
        SignalRLogger.LogToFile($"[SignalR] User autenticado: {Context.User?.Identity?.IsAuthenticated}");
        SignalRLogger.LogToFile($"[SignalR] User Claims Count: {Context.User?.Claims?.Count() ?? 0}");
        
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
                    SignalRLogger.LogToFile($"[SignalR] Claim: {claim.Type} = {claim.Value}");
                }
            }
        }
        else
        {
            // Si no está autenticado, intentar obtener el token de la query string y decodificarlo manualmente
            SignalRLogger.LogToFile("[SignalR] Usuario NO autenticado, intentando decodificar token manualmente");
            var httpContext = Context.GetHttpContext();
            var token = httpContext?.Request.Query["access_token"].ToString();
            
            if (!string.IsNullOrEmpty(token))
            {
                SignalRLogger.LogToFile($"[SignalR] Token encontrado en query string, decodificando...");
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(token);
                    
                    SignalRLogger.LogToFile($"[SignalR] Token decodificado - Claims Count: {jsonToken.Claims.Count()}");
                    
                    // Buscar userId en los claims del token
                    userId = jsonToken.Claims.FirstOrDefault(c => 
                        c.Type == ClaimTypes.NameIdentifier || 
                        c.Type == "nameid" || 
                        c.Type == "sub" ||
                        c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                    )?.Value;
                    
                    if (!string.IsNullOrEmpty(userId))
                    {
                        SignalRLogger.LogToFile($"[SignalR] UserId obtenido manualmente del token: {userId}");
                        
                        // Log todos los claims del token para debugging
                        foreach (var claim in jsonToken.Claims)
                        {
                            SignalRLogger.LogToFile($"[SignalR] Token Claim: {claim.Type} = {claim.Value}");
                        }
                    }
                    else
                    {
                        SignalRLogger.LogToFile("[SignalR] WARNING: No se encontró userId en los claims del token");
                    }
                }
                catch (Exception ex)
                {
                    SignalRLogger.LogToFile($"[SignalR] ERROR decodificando token manualmente: {ex.Message}");
                    SignalRLogger.LogToFile($"[SignalR] ERROR StackTrace: {ex.StackTrace}");
                }
            }
            else
            {
                SignalRLogger.LogToFile("[SignalR] WARNING: No se encontró token en query string");
            }
        }
        
        SignalRLogger.LogToFile($"[SignalR] UserId final encontrado: {userId ?? "NULL"}");

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            SignalRLogger.LogToFile($"[SignalR] Usuario agregado al grupo: user:{userId}");
        }
        else
        {
            SignalRLogger.LogToFile("[SignalR] WARNING: No se pudo obtener userId del token - usuario solo en grupo broadcast");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, "broadcast");
        SignalRLogger.LogToFile($"[SignalR] Usuario agregado al grupo: broadcast");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        SignalRLogger.LogToFile($"[SignalR] Usuario desconectado - ConnectionId: {Context.ConnectionId}");
        if (exception != null)
        {
            SignalRLogger.LogToFile($"[SignalR] Error en desconexión: {exception.Message}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
