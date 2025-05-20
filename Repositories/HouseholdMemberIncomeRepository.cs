using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories
{
    /// <summary>
    /// Repositorio para manejar las operaciones CRUD de ingresos de miembros del hogar
    /// </summary>
    public class HouseholdMemberIncomeRepository(DapperContext context, ILogger<HouseholdMemberIncomeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IHouseholdMemberIncomeRepository
    {
        private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<HouseholdMemberIncomeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

        /// <summary>
        /// Obtiene un ingreso de un miembro del hogar por su ID.
        /// </summary>
        /// <param name="id">El ID del ingreso a obtener.</param>
        /// <returns>El ingreso encontrado como objeto dinámico.</returns>
        /// <exception cref="Exception">Error al ejecutar la operación en la base de datos</exception>
        public async Task<dynamic> GetByIdAsync(int id)
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var result = await db.QueryFirstOrDefaultAsync<dynamic>("102_GetHouseholdMemberIncomeById", parameters, commandType: CommandType.StoredProcedure);

            return result;
        }

        /// <summary>
        /// Obtiene todos los ingresos de un miembro del hogar con paginación y filtrado opcional.
        /// </summary>
        /// <param name="take">El número de registros a devolver.</param>
        /// <param name="skip">El número de registros a omitir (para paginación).</param>
        /// <param name="name">Filtro opcional por nombre del ingreso.</param>
        /// <param name="alls">Indica si se deben devolver todos los registros sin paginación.</param>
        /// <returns>Lista de ingresos como objetos dinámicos.</returns>
        /// <exception cref="Exception">Error al ejecutar la operación en la base de datos</exception>
        public async Task<dynamic> GetAllAsync(int take, int skip, string name, bool alls)
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take);
            parameters.Add("@skip", skip);
            parameters.Add("@name", name);
            parameters.Add("@alls", alls);

            var result = await db.QueryAsync<dynamic>("102_GetHouseholdMemberIncomes", parameters, commandType: CommandType.StoredProcedure);

            return result;
        }

        /// <summary>
        /// Inserta un nuevo ingreso para un miembro del hogar.
        /// </summary>
        /// <param name="entity">DTO con los datos del ingreso a insertar.</param>
        /// <returns>El ID del nuevo ingreso insertado.</returns>
        /// <exception cref="Exception">Error al ejecutar la operación en la base de datos</exception>
        public async Task<int> InsertAsync(DTOHouseholdMemberIncome entity)
        {
            try
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@memberId", entity.MemberId);
                parameters.Add("@incomeTypeId", entity.IncomeTypeId);
                parameters.Add("@amount", entity.Amount);
                parameters.Add("@frequencyId", entity.FrequencyId);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync("102_InsertHouseholdMemberIncome", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@id");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar el ingreso del miembro del hogar");
                throw;
            }
        }

        /// <summary>
        /// Actualiza los datos de un ingreso existente de un miembro del hogar.
        /// </summary>
        /// <param name="entity">DTO con los datos actualizados del ingreso.</param>
        /// <returns>True si la actualización fue exitosa, False si no se encontró el registro.</returns>
        /// <exception cref="Exception">Error al ejecutar la operación en la base de datos</exception>
        public async Task<bool> UpdateAsync(DTOHouseholdMemberIncome entity)
        {
            try
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", entity.Id);
                parameters.Add("@incomeTypeId", entity.IncomeTypeId);
                parameters.Add("@amount", entity.Amount);
                parameters.Add("@frequencyId", entity.FrequencyId);

                var affectedRows = await db.ExecuteAsync("102_UpdateHouseholdMemberIncome", parameters, commandType: CommandType.StoredProcedure);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el ingreso del miembro del hogar");
                throw;
            }
        }

        /// <summary>
        /// Elimina un ingreso de miembro del hogar de forma lógica.
        /// </summary>
        /// <param name="id">ID del ingreso a eliminar.</param>
        /// <returns>True si se eliminó correctamente, False si no se encontró el registro.</returns>
        /// <exception cref="Exception">Error al ejecutar la operación en la base de datos</exception>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);

                var affectedRows = await db.ExecuteAsync("102_DeleteHouseholdMemberIncome", parameters, commandType: CommandType.StoredProcedure);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el ingreso del miembro del hogar");
                throw;
            }
        }
    }
}