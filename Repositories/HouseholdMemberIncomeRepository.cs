using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Api.Models;
using Dapper;

namespace Api.Repositories
{
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
        /// <returns>El ingreso encontrado.</returns>
        public async Task<dynamic> GetByIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>("102_GetHouseholdMemberIncomeById", parameters, commandType: CommandType.StoredProcedure);

            return result;
        }

        /// <summary>
        /// Obtiene todos los ingresos de un miembro del hogar.
        /// </summary>
        /// <param name="take">El número de ingresos a tomar.</param>
        /// <param name="skip">El número de ingresos a saltar.</param>
        /// <param name="name">El nombre del ingreso a buscar.</param>
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



        public async Task<int> InsertAsync(DTOHouseholdMemberIncome entity)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MemberId", entity.MemberId);
            parameters.Add("@IncomeTypeId", entity.IncomeTypeId);
            parameters.Add("@Amount", entity.Amount);
            parameters.Add("@FrequencyId", entity.FrequencyId);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbConnection.ExecuteAsync("102_InsertHouseholdMemberIncome", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@Id");
        }

        public async Task<bool> UpdateAsync(DTOHouseholdMemberIncome entity)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", entity.Id);
            parameters.Add("@IncomeTypeId", entity.IncomeTypeId);
            parameters.Add("@Amount", entity.Amount);
            parameters.Add("@FrequencyId", entity.FrequencyId);

            var affectedRows = await _dbConnection.ExecuteAsync("102_UpdateHouseholdMemberIncome", parameters, commandType: CommandType.StoredProcedure);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var affectedRows = await _dbConnection.ExecuteAsync("102_DeleteHouseholdMemberIncome", parameters, commandType: CommandType.StoredProcedure);
            return affectedRows > 0;
        }
    }
}