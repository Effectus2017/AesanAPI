using Api.Data;
using Api.Interfaces;
using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace Api.Services;

public class PasswordService(DapperContext context, ILogger<PasswordService> logger) : IPasswordService
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<PasswordService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


    /// <summary>
    /// Obtiene la contraseña temporal de un usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>La contraseña temporal o null si no existe</returns>
    public async Task<string> GetTemporaryPassword(string userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new { UserId = userId };
            var temporaryPassword = await dbConnection.QueryFirstOrDefaultAsync<string>("100_GetTemporaryPassword", param, commandType: CommandType.StoredProcedure);
            return temporaryPassword ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la contraseña temporal");
            throw new Exception("No se pudo obtener la contraseña temporal", ex);
        }
    }
}
