using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Api.Data;
using Api.Interfaces;
using Api.Models.DTO;
using Api.Models.Request;
using Api.Models;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories
{
    public class HouseholdMemberRepository(DapperContext context, ILoggingService logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IHouseholdMemberRepository
    {
        private readonly DapperContext _context = context;
        private readonly ILoggingService _logger = logger;
        private readonly IMemoryCache _cache = cache;
        private readonly ApplicationSettings _appSettings = appSettings.Value;

        /// <summary>
        /// Obtiene un HouseholdMember por su Id
        /// </summary>
        /// <param name="id">Id del HouseholdMember a obtener</param>
        /// <returns>DTOHouseholdMember con la información del miembro</returns>
        public async Task<dynamic> GetHouseholdMemberById(int id)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32, ParameterDirection.Input);
                var result = await dbConnection.QueryFirstOrDefaultAsync<DTOHouseholdMember>("101_GetHouseholdMemberById", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, $"Error al obtener HouseholdMember por Id: {id}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una lista paginada de HouseholdMembers para un Household
        /// </summary>
        /// <param name="id">Id del Household</param>
        /// <param name="take">Cantidad de registros a tomar</param>
        /// <param name="skip">Cantidad de registros a saltar</param>
        /// <returns>Lista de DTOHouseholdMember</returns>
        public async Task<dynamic> GetHouseholdMembers(int take, int skip, bool alls)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@take", take, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@skip", skip, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@alls", alls, DbType.Boolean, ParameterDirection.Input);
                var result = await dbConnection.QueryAsync<DTOHouseholdMember>("101_GetHouseholdMembers", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, $"Error al obtener HouseholdMembers para HouseholdId: {take}, {skip}, {alls}");
                throw;
            }
        }

        /// <summary>
        /// Inserta un nuevo HouseholdMember
        /// </summary>
        /// <param name="request">Datos del miembro a insertar</param>
        /// <returns>Id del miembro insertado</returns>
        public async Task<int> InsertHouseholdMember(HouseholdMemberRequest request)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", request.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@fullName", request.FullName, DbType.String, ParameterDirection.Input);
                parameters.Add("@birthDate", request.BirthDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@relationship", request.Relationship, DbType.String, ParameterDirection.Input);
                parameters.Add("@Gender", request.Gender, DbType.String, ParameterDirection.Input);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await dbConnection.ExecuteAsync("101_InsertHouseholdMember", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("@Id");
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "Error al insertar HouseholdMember");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un HouseholdMember existente
        /// </summary>
        /// <param name="request">Datos del miembro a actualizar</param>
        /// <returns>True si la actualización fue exitosa</returns>
        public async Task<bool> UpdateHouseholdMember(HouseholdMemberRequest request)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", request.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@fullName", request.FullName, DbType.String, ParameterDirection.Input);
                parameters.Add("@birthDate", request.BirthDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@relationship", request.Relationship, DbType.String, ParameterDirection.Input);
                parameters.Add("@gender", request.Gender, DbType.String, ParameterDirection.Input);
                var rows = await dbConnection.ExecuteAsync("101_UpdateHouseholdMember", parameters, commandType: CommandType.StoredProcedure);
                return rows > 0;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "Error al actualizar HouseholdMember");
                throw;
            }
        }

        /// <summary>
        /// Elimina un HouseholdMember por su Id
        /// </summary>
        /// <param name="id">Id del HouseholdMember a eliminar</param>
        /// <returns>True si la eliminación fue exitosa</returns>
        public async Task<bool> DeleteHouseholdMember(int id)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32, ParameterDirection.Input);
                var rows = await dbConnection.ExecuteAsync("101_DeleteHouseholdMember", parameters, commandType: CommandType.StoredProcedure);
                return rows > 0;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, $"Error al eliminar HouseholdMember Id: {id}");
                throw;
            }
        }
    }
}