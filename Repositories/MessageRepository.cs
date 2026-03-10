using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Models.Errors;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories
{
    public class MessageRepository(DapperContext context, ILogger<MessageRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IMessageRepository
    {
        private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<MessageRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

        public async Task<dynamic> GetMessageById(int id)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);

                var result = await dbConnection.QueryFirstOrDefaultAsync<Message>("100_GetMessageById", parameters, commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el mensaje con ID {MessageId}", id);
                throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener el mensaje", ex);
            }
        }

        public async Task<dynamic> GetAllMessagesFromDb(string? userId = null)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.String);

                var result = await dbConnection.QueryAsync<Message>("100_GetAllMessages", parameters, commandType: CommandType.StoredProcedure);

                return result ?? Enumerable.Empty<Message>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los mensajes");
                throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener todos los mensajes", ex);
            }
        }

        public async Task<Message> InsertMessage(MessageRequest messageRequest)
        {
            try
            {
                _logger.LogInformation("Insertando nuevo mensaje");

                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@icon", messageRequest.Icon, DbType.String, ParameterDirection.Input);
                parameters.Add("@image", messageRequest.Image, DbType.String, ParameterDirection.Input);
                parameters.Add("@title", messageRequest.Title, DbType.String, ParameterDirection.Input);
                parameters.Add("@description", messageRequest.Description, DbType.String, ParameterDirection.Input);
                parameters.Add("@time", DateTime.UtcNow, DbType.DateTime2, ParameterDirection.Input);
                parameters.Add("@link", messageRequest.Link, DbType.String, ParameterDirection.Input);
                parameters.Add("@useRouter", messageRequest.UseRouter, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@read", false, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@userId", messageRequest.UserId, DbType.String, ParameterDirection.Input);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_InsertMessage", parameters, commandType: CommandType.StoredProcedure);

                var newId = parameters.Get<int>("@id");

                var getParams = new DynamicParameters();
                getParams.Add("@id", newId, DbType.Int32);
                var created = await dbConnection.QueryFirstOrDefaultAsync<Message>(
                    "100_GetMessageById", getParams, commandType: CommandType.StoredProcedure);

                if (created == null)
                {
                    throw new InvalidOperationException("No fue posible recuperar el mensaje insertado.");
                }

                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar mensaje");
                throw;
            }
        }

        public async Task<bool> UpdateMessage(MessageRequest messageRequest)
        {
            try
            {
                if (!messageRequest.Id.HasValue)
                {
                    _logger.LogWarning("No se puede actualizar un mensaje sin ID");
                    return false;
                }

                _logger.LogInformation("Actualizando mensaje con ID {MessageId}", messageRequest.Id.Value);

                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", messageRequest.Id.Value, DbType.Int32);
                parameters.Add("@icon", messageRequest.Icon, DbType.String);
                parameters.Add("@image", messageRequest.Image, DbType.String);
                parameters.Add("@title", messageRequest.Title, DbType.String);
                parameters.Add("@description", messageRequest.Description, DbType.String);
                parameters.Add("@link", messageRequest.Link, DbType.String);
                parameters.Add("@useRouter", messageRequest.UseRouter, DbType.Boolean);
                parameters.Add("@userId", messageRequest.UserId, DbType.String);
                var rowsAffected = await dbConnection.ExecuteAsync("100_UpdateMessage", parameters, commandType: CommandType.StoredProcedure);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el mensaje con ID {MessageId}", messageRequest.Id);
                throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al actualizar el mensaje", ex);
            }
        }

        public async Task<bool> DeleteMessage(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando mensaje con ID {MessageId}", id);

                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await dbConnection.ExecuteAsync("100_DeleteMessage", parameters, commandType: CommandType.StoredProcedure);

                var rowsAffected = parameters.Get<int>("@rowsAffected");

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el mensaje con ID {MessageId}", id);
                throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al eliminar el mensaje", ex);
            }
        }

        public async Task<bool> MarkAllMessagesAsRead(string? userId = null)
        {
            try
            {
                _logger.LogInformation("Marcando todos los mensajes como leídos para usuario {UserId}", userId);

                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.String);
                parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await dbConnection.ExecuteAsync("100_MarkAllMessagesAsRead", parameters, commandType: CommandType.StoredProcedure);

                var rowsAffected = parameters.Get<int>("@rowsAffected");

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar todos los mensajes como leídos para usuario {UserId}", userId);
                throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al marcar todos los mensajes como leídos", ex);
            }
        }

        public async Task<bool> MarkMessageAsRead(int id)
        {
            try
            {
                _logger.LogInformation("Marcando mensaje como leído con ID {MessageId}", id);

                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await dbConnection.ExecuteAsync("100_MarkMessageAsRead", parameters, commandType: CommandType.StoredProcedure);

                var rowsAffected = parameters.Get<int>("@rowsAffected");

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar mensaje como leído con ID {MessageId}", id);
                throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al marcar mensaje como leído", ex);
            }
        }

        public async Task<int> GetUnreadMessageCount(string? userId = null)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.String);

                var result = await dbConnection.ExecuteScalarAsync<int>("100_GetUnreadMessageCount", parameters, commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el conteo de mensajes no leídos para usuario {UserId}", userId);
                throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener el conteo de mensajes no leídos", ex);
            }
        }
    }
}