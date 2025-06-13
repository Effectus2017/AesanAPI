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
    public class HouseholdRepository(DapperContext context, ILoggingService logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IHouseholdRepository
    {
        private readonly DapperContext _context = context;
        private readonly ILoggingService _logger = logger;
        private readonly IMemoryCache _cache = cache;
        private readonly ApplicationSettings _appSettings = appSettings.Value;

        /// <summary>
        /// Obtiene un Household por su Id
        /// </summary>
        /// <param name="id">Id del Household a obtener</param>
        /// <returns>DTOHousehold con la información del Household</returns>
        public async Task<dynamic> GetHouseholdById(int id)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
                var result = await dbConnection.QueryFirstOrDefaultAsync<DTOHousehold>("101_GetHouseholdById", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, $"Error al obtener Household por Id: {id}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una lista paginada de Households
        /// </summary>
        /// <param name="take">Cantidad de registros a tomar</param>
        /// <param name="skip">Cantidad de registros a saltar</param>
        /// <returns>Lista de DTOHousehold</returns>
        public async Task<dynamic> GetHouseholds(int take, int skip)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@take", take, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@skip", skip, DbType.Int32, ParameterDirection.Input);
                var result = await dbConnection.QueryAsync<DTOHousehold>("101_GetHouseholds", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "Error al obtener Households");
                throw;
            }
        }

        /// <summary>
        /// Inserta un nuevo Household
        /// </summary>
        /// <param name="request">Datos del Household a insertar</param>
        /// <returns>Id del Household insertado</returns>
        public async Task<int> InsertHousehold(HouseholdRequest request)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@street", request.Street, DbType.String, ParameterDirection.Input);
                parameters.Add("@apartment", request.Apartment, DbType.String, ParameterDirection.Input);
                parameters.Add("@cityId", request.CityId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@regionId", request.RegionId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@zipCode", request.ZipCode, DbType.String, ParameterDirection.Input);
                parameters.Add("@phone", request.Phone, DbType.String, ParameterDirection.Input);
                parameters.Add("@email", request.Email, DbType.String, ParameterDirection.Input);
                parameters.Add("@completedBy", request.CompletedBy, DbType.String, ParameterDirection.Input);
                parameters.Add("@completedDate", request.CompletedDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await dbConnection.ExecuteAsync("101_InsertHousehold", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("@id");
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "Error al insertar Household");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Household existente
        /// </summary>
        /// <param name="request">Datos del Household a actualizar</param>
        /// <returns>True si la actualización fue exitosa</returns>
        public async Task<bool> UpdateHousehold(HouseholdRequest request)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", request.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@street", request.Street, DbType.String, ParameterDirection.Input);
                parameters.Add("@apartment", request.Apartment, DbType.String, ParameterDirection.Input);
                parameters.Add("@cityId", request.CityId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@regionId", request.RegionId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@zipCode", request.ZipCode, DbType.String, ParameterDirection.Input);
                parameters.Add("@phone", request.Phone, DbType.String, ParameterDirection.Input);
                parameters.Add("@email", request.Email, DbType.String, ParameterDirection.Input);
                parameters.Add("@completedBy", request.CompletedBy, DbType.String, ParameterDirection.Input);
                parameters.Add("@completedDate", request.CompletedDate, DbType.DateTime, ParameterDirection.Input);
                var rows = await dbConnection.ExecuteAsync("101_UpdateHousehold", parameters, commandType: CommandType.StoredProcedure);
                return rows > 0;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, "Error al actualizar Household");
                throw;
            }
        }

        /// <summary>
        /// Elimina un Household por su Id
        /// </summary>
        /// <param name="id">Id del Household a eliminar</param>
        /// <returns>True si la eliminación fue exitosa</returns>
        public async Task<bool> DeleteHousehold(int id)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32, ParameterDirection.Input);
                var rows = await dbConnection.ExecuteAsync("101_DeleteHousehold", parameters, commandType: CommandType.StoredProcedure);
                return rows > 0;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, $"Error al eliminar Household Id: {id}");
                throw;
            }
        }

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
        public async Task<dynamic> GetHouseholdMembers(int take, int skip, int memberId)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@take", take, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@skip", skip, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@memberId", memberId, DbType.Int32, ParameterDirection.Input);
                var result = await dbConnection.QueryAsync<DTOHouseholdMember>("101_GetHouseholdMembers", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, $"Error al obtener HouseholdMembers para HouseholdId: {memberId}");
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
                var p = new DynamicParameters();
                p.Add("@id", request.Id, DbType.Int32, ParameterDirection.Input);
                p.Add("@fullName", request.FullName, DbType.String, ParameterDirection.Input);
                p.Add("@birthDate", request.BirthDate, DbType.DateTime, ParameterDirection.Input);
                p.Add("@relationship", request.Relationship, DbType.String, ParameterDirection.Input);
                p.Add("@Gender", request.Gender, DbType.String, ParameterDirection.Input);
                p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await dbConnection.ExecuteAsync("101_InsertHouseholdMember", p, commandType: CommandType.StoredProcedure);
                return p.Get<int>("@Id");
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
                parameters.Add("@Id", request.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@FullName", request.FullName, DbType.String, ParameterDirection.Input);
                parameters.Add("@BirthDate", request.BirthDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Relationship", request.Relationship, DbType.String, ParameterDirection.Input);
                parameters.Add("@Gender", request.Gender, DbType.String, ParameterDirection.Input);
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
                parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
                var rows = await dbConnection.ExecuteAsync("101_DeleteHouseholdMember", parameters, commandType: CommandType.StoredProcedure);
                return rows > 0;
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, $"Error al eliminar HouseholdMember Id: {id}");
                throw;
            }
        }

        Task<dynamic> IHouseholdRepository.GetAllHouseholds(int take, int skip, bool alls)
        {
            throw new NotImplementedException();
        }

        Task<bool> IHouseholdRepository.InsertHousehold(HouseholdRequest request)
        {
            throw new NotImplementedException();
        }

        Task<bool> IHouseholdRepository.UpdateHousehold(HouseholdRequest request)
        {
            throw new NotImplementedException();
        }



    }
}