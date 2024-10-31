using System.Data;
using Api.Data;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class ProgramRepository(DapperContext context, ILogger<ProgramRepository> logger) : IProgramRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<ProgramRepository> _logger = logger;

    /// <summary>
    /// Obtiene todos los programas de la base de datos
    /// </summary>
    /// <param name="take">El número de programas a obtener</param>
    /// <param name="skip">El número de programas a saltar</param>
    /// <param name="name">El nombre del programa</param>
    /// <param name="alls">Si se deben obtener todos los programas</param>
    /// <returns>Los programas</returns>
    public async Task<dynamic> GetAllProgramsFromDb(int take, int skip, string name, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los programas de la base de datos");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new { take, skip, name, alls };

            var result = await dbConnection.QueryMultipleAsync("100_GetPrograms", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var programs = result.Read<dynamic>().Select(item => new DTOProgram
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description
            }).ToList();

            var count = result.Read<int>().Single();

            return new { data = programs, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los programas");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un programa por su ID
    /// </summary>
    /// <param name="id">El ID del programa</param>
    /// <returns>El programa</returns>
    public async Task<dynamic> GetProgramById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo programa por ID: {Id}", id);

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new { Id = id };

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>("100_GetProgramById", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            return new DTOProgram
            {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el programa");
            throw new Exception(ex.Message);
        }
    }
}
