using System.Data;
using System.Linq;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

/// <summary>
/// Repositorio de agencias
/// </summary>
public class AgencyRepository(IEmailService emailService, IPasswordService passwordService, IAgencyUsersRepository agencyUsersRepository, DapperContext context, ILoggingService loggingService, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : IAgencyRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly IPasswordService _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
    private readonly IAgencyUsersRepository _agencyUsersRepository = agencyUsersRepository ?? throw new ArgumentNullException(nameof(agencyUsersRepository));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene una agencia por su ID
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    public async Task<dynamic> GetAgencyById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32, ParameterDirection.Input);
            // Nota: userId es opcional en el nuevo SP, si no se proporciona permite acceso (comportamiento legacy)

            var result = await dbConnection.QueryMultipleAsync("114_GetAgencyById", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                var error = new Exception($"No se encontraron resultados para la agencia ID: {id}");
                await _logger.LogError(error, "No se encontraron resultados");
                return null;
            }

            var _agencyDynamic = await result.ReadFirstOrDefaultAsync<dynamic>();

            if (_agencyDynamic == null)
            {
                var error = new Exception($"No se encontró la agencia con ID: {id}");
                await _logger.LogError(error, "No se encontró la agencia");
                return null;
            }

            var agency = _mappingService.MapAgency(_agencyDynamic);

            var _agenciesPrograms = await result.ReadAsync<dynamic>();

            if (_agenciesPrograms.Any())
            {
                agency.Programs = _mappingService.MapPrograms(_agenciesPrograms);
            }

            // Leer el tercer result set: Usuario monitor asociado
            var _agenciesMonitors = await result.ReadAsync<dynamic>();

            if (_agenciesMonitors.Any())
            {
                var monitorData = _agenciesMonitors.FirstOrDefault();
                if (monitorData != null)
                {
                    agency.Monitor = _mappingService.MapStaffDetails(monitorData);
                }
            }

            // Leer el cuarto result set: Usuario owner (que creó la agencia)
            var _agenciesUser = await result.ReadAsync<dynamic>();

            if (_agenciesUser.Any())
            {
                var user = _agenciesUser.FirstOrDefault();

                if (user != null)
                {
                    // Mapear correctamente los datos del owner con la información de Position
                    agency.User = _mappingService.MapStaffDetails(user);
                }
            }

            // Leer el quinto result set: Funciones de autoridad de la Junta de Directores (BoardExecutiveAuthority)
            var _boardExecutiveAuthority = await result.ReadAsync<dynamic>();

            if (_boardExecutiveAuthority.Any() && agency.Inscription != null)
            {
                var boardAuthorityIds = _boardExecutiveAuthority.Select(x => (int)x.OptionSelectionId).ToList();
                var boardAuthorityOptions = _boardExecutiveAuthority.Select(x => _mappingService.MapOptionSelection(
                    (int?)x.Id,
                    x.Name?.ToString(),
                    x.NameEN?.ToString(),
                    x.OptionKey?.ToString()
                )).Where(x => x != null).Cast<DTOOptionSelection>().ToList();
                
                agency.Inscription.BoardExecutiveAuthorityIds = boardAuthorityIds;
                agency.Inscription.BoardExecutiveAuthority = boardAuthorityOptions.Any() ? boardAuthorityOptions : null;
            }

            return agency;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error getting agency by id {id}: {ex.Message}");
            throw new Exception($"Error al obtener la agencia con ID {id}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene una agencia por su ID y el ID del usuario
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    public async Task<dynamic> GetAgencyByIdAndUserId(int agencyId, string userId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo datos de la base de datos para agencia {agencyId} y usuario {userId}");
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId);
            parameters.Add("@userId", userId);
            
            // Usar nuevo SP con nueva lógica de acceso
            var result = await dbConnection.QueryMultipleAsync("113_GetAgencyByIdAndUserId", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var _agencyDynamic = await result.ReadFirstOrDefaultAsync<dynamic>();

            if (_agencyDynamic == null)
            {
                return null;
            }

            var agency = _mappingService.MapAgency(_agencyDynamic);

            var _agenciesPrograms = await result.ReadAsync<dynamic>();

            if (_agenciesPrograms != null && _agenciesPrograms.Any())
            {
                agency.Programs = _mappingService.MapPrograms(_agenciesPrograms);
            }

            // Leer el tercer result set: Usuarios que hicieron appointments (puede estar vacío)
            var _appointmentUsers = await result.ReadAsync<dynamic>();

            // Leer el cuarto result set: Funciones de autoridad de la Junta de Directores (BoardExecutiveAuthority)
            var _boardExecutiveAuthority = await result.ReadAsync<dynamic>();

            if (_boardExecutiveAuthority.Any() && agency.Inscription != null)
            {
                var boardAuthorityIds = _boardExecutiveAuthority.Select(x => (int)x.OptionSelectionId).ToList();
                var boardAuthorityOptions = _boardExecutiveAuthority.Select(x => _mappingService.MapOptionSelection(
                    (int?)x.Id,
                    x.Name?.ToString(),
                    x.NameEN?.ToString(),
                    x.OptionKey?.ToString()
                )).Where(x => x != null).Cast<DTOOptionSelection>().ToList();
                
                agency.Inscription.BoardExecutiveAuthorityIds = boardAuthorityIds;
                agency.Inscription.BoardExecutiveAuthority = boardAuthorityOptions.Any() ? boardAuthorityOptions : null;
            }

            _logger.LogInformation($"Datos obtenidos de la base de datos para agencia {agencyId} y usuario {userId}");
            return agency!;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error getting agency by id {agencyId} and user id {userId}: {ex.Message}");
            throw new Exception($"Error al obtener la agencia con ID {agencyId} y usuario {userId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todas las agencias de la base de datos
    /// </summary>
    public async Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, string? userId, bool alls, bool isList, bool? isPropietary, string? userFirstName, string? statusName, string? monitorFirstName, DateTime? createdAtFrom, DateTime? createdAtTo, long? uieNumber, int? einNumber, long? sdrNumber)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@take", take);
            param.Add("@skip", skip);
            param.Add("@name", name);
            param.Add("@regionId", regionId == 0 ? null : regionId);
            param.Add("@cityId", cityId == 0 ? null : cityId);
            param.Add("@programId", programId == 0 ? null : programId);
            param.Add("@statusId", statusId == 0 ? null : statusId);
            param.Add("@userId", userId);
            param.Add("@alls", alls);
            param.Add("@isPropietary", isPropietary);
            param.Add("@userFirstName", userFirstName);
            param.Add("@statusName", statusName);
            param.Add("@monitorFirstName", monitorFirstName);
            param.Add("@createdatfrom", createdAtFrom);
            param.Add("@createdatto", createdAtTo);
            param.Add("@uieNumber", uieNumber);
            param.Add("@einNumber", einNumber);
            param.Add("@sdrNumber", sdrNumber);

            // Usar nuevo SP con nueva lógica de acceso
            if (isList)
            {
                using var result = await dbConnection.QueryMultipleAsync("118_GetAgencies", param, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return new List<dynamic>();
                }

                var agencies = result.Read<dynamic>().ToList();
                var data = _mappingService.MapAgencyList(agencies).ToList();
                return data;
            }
            else
            {
                // Variables para almacenar los resultados
                List<dynamic> agencies = [];
                List<DTOProgram> agenciesPrograms = [];
                List<DTOStaff> agenciesMonitors = [];
                List<DTOStaff> agenciesOwners = [];
                int count = 0;

                using var result = await dbConnection.QueryMultipleAsync("118_GetAgencies", param, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return new { data = Array.Empty<dynamic>(), count = 0 };
                }

                // Leer todos los conjuntos de resultados de manera segura
                if (!result.IsConsumed)
                {
                    agencies = result.Read<dynamic>().ToList();
                }

                if (!result.IsConsumed)
                {
                    agenciesPrograms = result.Read<DTOProgram>().ToList();
                }

                if (!result.IsConsumed)
                {
                    agenciesMonitors = result.Read<DTOStaff>().ToList();
                }

                if (!result.IsConsumed)
                {
                    agenciesOwners = result.Read<DTOStaff>().ToList();
                }

                if (!result.IsConsumed)
                {
                    count = result.ReadFirstOrDefault<int>();
                }

                // Procesar los datos después de que el GridReader se haya cerrado
                var data = agencies.Select(_mappingService.MapAgency).ToList();

                // Asignar programas a cada agencia
                if (agenciesPrograms != null && agenciesPrograms.Count != 0)
                {
                    foreach (var agency in data)
                    {
                        agency.Programs = agenciesPrograms.ToList();
                    }
                }

                // Asignar monitors a cada agencia
                if (agenciesMonitors != null && agenciesMonitors.Count != 0)
                {
                    foreach (var agency in data)
                    {
                        var monitorData = agenciesMonitors.Where(am => am.AgencyId == agency.Id).FirstOrDefault();
                        if (monitorData != null)
                        {
                            agency.Monitor = monitorData;
                        }
                    }
                }

                // Asignar owners a cada agencia
                if (agenciesOwners != null && agenciesOwners.Count != 0)
                {
                    foreach (var agency in data)
                    {
                        var ownerData = agenciesOwners.Where(ao => ao.AgencyId == agency.Id).FirstOrDefault();
                        if (ownerData != null)
                        {
                            agency.User = ownerData;
                        }
                    }
                }

                return new { data, count };
            }


        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error getting all agencies from database: {ex.Message}");
            throw new Exception($"Error al obtener las agencias de la base de datos: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene los programas de una agencia por el ID del usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>Los programas de la agencia</returns>
    public async Task<dynamic> GetAgencyProgramsByUserId(string userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            var result = await dbConnection.QueryAsync<DTOProgram>("100_GetAgencyProgramsByUserId", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error getting agency programs by user id {userId}: {ex.Message}");
            throw new Exception($"Error al obtener los programas de la agencia para el usuario {userId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Inserta una nueva agencia en la base de datos
    /// </summary>
    /// <param name="agencyRequest">Objeto con los datos de la agencia a insertar</param>
    /// <returns>El ID de la agencia insertada</returns>
    public async Task<int> InsertAgency(AgencyRequest agencyRequest)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@name", agencyRequest.Name);
            parameters.Add("@agencyStatusId", agencyRequest.StatusId);
            parameters.Add("@sdrNumber", agencyRequest.SdrNumber);
            parameters.Add("@uieNumber", agencyRequest.UieNumber);
            parameters.Add("@EinNumber", agencyRequest.EinNumber);
            parameters.Add("@address", agencyRequest.Address);
            parameters.Add("@zipCode", agencyRequest.ZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@phone", agencyRequest.Phone);
            parameters.Add("@email", agencyRequest.Email);
            parameters.Add("@cityId", agencyRequest.CityId);
            parameters.Add("@regionId", agencyRequest.RegionId);
            parameters.Add("@postalAddress", agencyRequest.PostalAddress);
            parameters.Add("@postalZipCode", agencyRequest.PostalZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@postalCityId", agencyRequest.PostalCityId);
            parameters.Add("@postalRegionId", agencyRequest.PostalRegionId);
            parameters.Add("@latitude", Math.Round(agencyRequest.Latitude, 2));
            parameters.Add("@longitude", Math.Round(agencyRequest.Longitude, 2));
            parameters.Add("@imageUrl", agencyRequest.ImageUrl);
            parameters.Add("@isActive", agencyRequest.IsActive);
            parameters.Add("@isListable", agencyRequest.IsListable);
            parameters.Add("@agencyCode", agencyRequest.AgencyCode);
            parameters.Add("@isPropietary", false);
            parameters.Add("@isRecurrent", false);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("111_InsertAgency", parameters, commandType: CommandType.StoredProcedure);

            var agencyId = parameters.Get<int>("@id");

            if (agencyId <= 0)
            {
                throw new Exception("Error al insertar la agencia");
            }

            // Insertar la solicitud de participación de la Agencia
            var agencyInscriptionId = await InsertAgencyInscription(
                agencyId,
                agencyRequest.NonProfit,
                agencyRequest.FederalFundsDenied,
                agencyRequest.FederalFundsDeniedReason, // Nuevo argumento
                agencyRequest.StateFundsDenied,
                agencyRequest.StateFundsDeniedReason,
                agencyRequest.BasicEducationRegistry,
                agencyRequest.ExtendedHours,
                agencyRequest.ServicesOfferedSince,
                agencyRequest.TaxExemptionStatusId,
                agencyRequest.TaxExemptionTypeId,
                agencyRequest.PublicAllianceContractId,
                agencyRequest.NationalYouthProgram,
                agencyRequest.IsDayCareHomeId,
                agencyRequest.ParticipatesInHeadStartProgramId,
                agencyRequest.BoardMeetingsPerYear,
                agencyRequest.BoardMeetsRegularly
            );

            // Insertar funciones de autoridad de la Junta de Directores (BoardExecutiveAuthority)
            if (agencyRequest.BoardExecutiveAuthority != null && agencyRequest.BoardExecutiveAuthority.Count > 0)
            {
                await InsertAgencyInscriptionBoardExecutiveAuthority(agencyInscriptionId, agencyRequest.BoardExecutiveAuthority);
            }

            // Asignar programas a la agencia
            if (agencyRequest.Programs != null && agencyRequest.Programs.Count > 0)
            {
                foreach (var programId in agencyRequest.Programs)
                {
                    await InsertAgencyProgram(agencyId, programId);
                }
            }

            // Invalidar caché
            await InvalidateCache(agencyId);

            return agencyId;
        }
        catch (Exception ex)
        {
            // Verificar si es un error de duplicado desde el stored procedure
            // Los stored procedures lanzan errores con códigos 50001 (IUE), 50002 (EIN), 50003 (SDR)
            if (ex.Message.Contains("IUE") || ex.Message.Contains("EIN") || ex.Message.Contains("SDR") || 
                ex.Message.Contains("ya está registrado"))
            {
                await _logger.LogError(ex, $"Intento de insertar agencia con identificador duplicado: {ex.Message}");
                // Re-lanzar el mensaje original del stored procedure que ya es descriptivo
                throw new Exception(ex.Message, ex);
            }
            
            // Verificar si es una violación de constraint UNIQUE de la base de datos
            // (por si alguien inserta directamente sin pasar por el stored procedure)
            if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate key") || 
                ex.Message.Contains("UK_Agency_UieNumber") || ex.Message.Contains("UK_Agency_EinNumber") || 
                ex.Message.Contains("UK_Agency_SdrNumber"))
            {
                string errorMessage = "El identificador proporcionado ya está registrado en el sistema.";
                
                if (ex.Message.Contains("UieNumber") || ex.Message.Contains("IUE"))
                {
                    errorMessage = $"El Identificador Único de Entidad (IUE) ya está registrado en el sistema.";
                }
                else if (ex.Message.Contains("EinNumber") || ex.Message.Contains("EIN"))
                {
                    errorMessage = $"El Número de Seguro Social Patronal (EIN) ya está registrado en el sistema.";
                }
                else if (ex.Message.Contains("SdrNumber") || ex.Message.Contains("SDR"))
                {
                    errorMessage = $"El Número de Registro del Departamento de Estado (SDR) ya está registrado en el sistema.";
                }
                
                await _logger.LogError(ex, $"Intento de insertar agencia con identificador duplicado (constraint): {errorMessage}");
                throw new Exception(errorMessage, ex);
            }
            
            await _logger.LogError(ex, $"Error inserting agency: {ex.Message}");
            throw new Exception($"Error al insertar la agencia: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Inserta una inscripción de agencia en la base de datos
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="nonProfit">Si es sin fines de lucro</param>
    /// <param name="federalFundsDenied">Fondos federales denegados</param>
    /// <param name="stateFundsDenied">Fondos estatales denegados</param>
    /// <param name="stateFundsDeniedReason">Razón por la cual fue descalificado o denegado de fondos estatales</param>
    /// <param name="basicEducationRegistry">Registro de educación básica</param>
    /// <param name="taxExemptionStatus">Estado de exención de impuestos</param>
    /// <param name="taxExemptionType">Tipo de exención de impuestos</param>
    /// <returns>El Id de la inscripción insertada</returns>
    public async Task<int> InsertAgencyInscription(int agencyId, bool nonProfit, bool federalFundsDenied, string? federalFundsDeniedReason, bool stateFundsDenied, string? stateFundsDeniedReason, bool basicEducationRegistry, bool extendedHours, DateTime? servicesOfferedSince, int taxExemptionStatusId, int taxExemptionTypeId, int? publicAllianceContractId, bool nationalYouthProgram, int? isDayCareHomeId, int? participatesInHeadStartProgramId = null, int? boardMeetingsPerYear = null, bool? boardMeetsRegularly = null)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId);
            parameters.Add("@nonProfit", nonProfit);
            parameters.Add("@federalFundsDenied", federalFundsDenied);
            parameters.Add("@federalFundsDeniedReason", federalFundsDeniedReason); // Nuevo parámetro
            parameters.Add("@stateFundsDenied", stateFundsDenied);
            parameters.Add("@stateFundsDeniedReason", stateFundsDeniedReason);
            parameters.Add("@basicEducationRegistry", basicEducationRegistry);
            parameters.Add("@extendedHours", extendedHours);
            parameters.Add("@servicesOfferedSince", servicesOfferedSince);
            parameters.Add("@taxExemptionStatusId", taxExemptionStatusId);
            parameters.Add("@taxExemptionTypeId", taxExemptionTypeId);
            parameters.Add("@publicAllianceContractId", publicAllianceContractId);
            parameters.Add("@nationalYouthProgram", nationalYouthProgram);
            parameters.Add("@isDayCareHomeId", isDayCareHomeId);
            parameters.Add("@participatesInHeadStartProgramId", participatesInHeadStartProgramId);
            parameters.Add("@boardMeetingsPerYear", boardMeetingsPerYear);
            parameters.Add("@boardMeetsRegularly", boardMeetsRegularly);

            // Deadline to complete the registration of the Sites
            // Tomar valor desde AppSettings que es un numero de días y convertir a date-time
            var deadlineToCompleteRegistration = DateTime.Now.AddDays(_appSettings.DeadlineToCompleteRegistration);
            parameters.Add("@deadlineToCompleteRegistration", deadlineToCompleteRegistration);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("112_InsertAgencyInscription", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@id");
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error inserting agency inscription for agency {agencyId}: {ex.Message}");
            throw new Exception($"Error al insertar la inscripción de la agencia {agencyId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Inserta múltiples funciones de autoridad de la Junta de Directores para una inscripción de agencia
    /// </summary>
    /// <param name="agencyInscriptionId">ID de la inscripción de la agencia</param>
    /// <param name="optionSelectionIds">Lista de IDs de opciones de selección (OptionKey = 'boardExecutiveAuthority')</param>
    /// <param name="connection">Conexión de base de datos (opcional)</param>
    /// <param name="transaction">Transacción de base de datos (opcional)</param>
    /// <returns>True si se insertaron correctamente</returns>
    private async Task<bool> InsertAgencyInscriptionBoardExecutiveAuthority(int agencyInscriptionId, List<int> optionSelectionIds, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@agencyInscriptionId", agencyInscriptionId, DbType.Int32);
            parameters.Add("@optionSelectionIds", string.Join(",", optionSelectionIds), DbType.String);

            await dbConnection.ExecuteAsync("100_InsertAgencyInscriptionBoardExecutiveAuthority", parameters, transaction, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error al insertar funciones de autoridad de la Junta de Directores para la inscripción {agencyInscriptionId}: {ex.Message}");
            throw new Exception($"Error al insertar funciones de autoridad de la Junta de Directores: {ex.Message}", ex);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Asigna una agencia a un programa
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="programId">Id del programa</param>
    /// <returns>True si se asignó correctamente</returns>
    public async Task<bool> InsertAgencyProgram(int agencyId, int programId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId);
            parameters.Add("@programId", programId);

            var rowsAffected = await dbConnection.QueryFirstOrDefaultAsync<int>("100_InsertAgencyProgram", parameters, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error inserting agency program for agency {agencyId} and program {programId}: {ex.Message}");
            throw new Exception($"Error al insertar el programa {programId} para la agencia {agencyId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza los datos de una agencia
    /// </summary>
    /// <param name="agencyId">Id de la agencia a actualizar</param>
    /// <param name="agencyRequest">Objeto con los nuevos datos de la agencia</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateAgency(int agencyId, AgencyRequest agencyRequest)
    {
        try
        {
            using IDbConnection connection = _context.CreateConnection();

            // Obtener la agencia actual para comparar el monitor
            var currentAgency = await GetAgencyById(agencyId);

            if (currentAgency == null)
            {
                throw new ArgumentNullException(nameof(currentAgency), "La agencia actual no puede ser nula");
            }

            int? currentMonitorId = currentAgency.Monitor?.Id;

            var parameters = new DynamicParameters();
            parameters.Add("@id", agencyId);
            parameters.Add("@name", agencyRequest.Name);
            parameters.Add("@agencyStatusId", agencyRequest.StatusId);
            parameters.Add("@sdrNumber", agencyRequest.SdrNumber);
            parameters.Add("@uieNumber", agencyRequest.UieNumber);
            parameters.Add("@einNumber", agencyRequest.EinNumber);
            parameters.Add("@address", agencyRequest.Address);
            parameters.Add("@zipCode", agencyRequest.ZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@cityId", agencyRequest.CityId);
            parameters.Add("@regionId", agencyRequest.RegionId);
            parameters.Add("@latitude", Math.Round(agencyRequest.Latitude, 2));
            parameters.Add("@longitude", Math.Round(agencyRequest.Longitude, 2));
            parameters.Add("@postalAddress", agencyRequest.PostalAddress);
            parameters.Add("@postalZipCode", agencyRequest.PostalZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@postalCityId", agencyRequest.PostalCityId);
            parameters.Add("@postalRegionId", agencyRequest.PostalRegionId);
            parameters.Add("@phone", agencyRequest.Phone);
            parameters.Add("@imageURL", agencyRequest.ImageUrl);
            parameters.Add("@email", agencyRequest.Email);
            parameters.Add("@isRecurrent", agencyRequest.IsRecurrent);

            var rowsAffected = await connection.ExecuteAsync("111_UpdateAgency", parameters, commandType: CommandType.StoredProcedure);

            // Verificar si hay un nuevo monitor asignado y es diferente al actual
            if (agencyRequest.MonitorId.HasValue && currentMonitorId != agencyRequest.MonitorId)
            {
                // Si había un monitor previo, des asignar el monitor primero
                if (currentMonitorId.HasValue)
                {
                    _logger.LogInformation($"Des asignando monitor anterior {currentMonitorId} de la agencia {agencyId}");
                    await _agencyUsersRepository.UnassignAgencyFromUser(currentMonitorId.ToString(), agencyId);

                    // Enviar correo de des asignación al monitor anterior
                    var previousMonitor = new DTOUser
                    {
                        Id = currentMonitorId.ToString(),
                        FirstName = currentAgency.Monitor.FirstName,
                        FatherLastName = currentAgency.Monitor.FatherLastName,
                        Email = currentAgency.Monitor.Email
                    };
                    await _emailService.SendAgencyUnassignmentEmail(previousMonitor, currentAgency);
                }

                // Asignar el nuevo monitor
                _logger.LogInformation($"Asignando nuevo monitor {agencyRequest.MonitorId} a la agencia {agencyId}");
                // Calcular AgencyAssignmentType según el rol del monitor/coordinador
                string agencyAssignmentType = await _agencyUsersRepository.CalculateAgencyAssignmentTypeFromRole(agencyRequest.MonitorId.ToString());
                await _agencyUsersRepository.AssignAgencyToUser(agencyRequest.MonitorId.ToString(), agencyId, agencyRequest.AssignedBy, agencyAssignmentType);
            }

            // Invalidar caché
            if (rowsAffected > 0)
            {
                await InvalidateCache(agencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            // Verificar si es un error de duplicado desde el stored procedure
            // Los stored procedures lanzan errores con códigos 50001 (IUE), 50002 (EIN), 50003 (SDR)
            if (ex.Message.Contains("IUE") || ex.Message.Contains("EIN") || ex.Message.Contains("SDR") || 
                ex.Message.Contains("ya está registrado") || ex.Message.Contains("otra agencia"))
            {
                await _logger.LogError(ex, $"Intento de actualizar agencia {agencyId} con identificador duplicado: {ex.Message}");
                // Re-lanzar el mensaje original del stored procedure que ya es descriptivo
                throw new Exception(ex.Message, ex);
            }
            
            // Verificar si es una violación de constraint UNIQUE de la base de datos
            // (por si alguien actualiza directamente sin pasar por el stored procedure)
            if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate key") || 
                ex.Message.Contains("UK_Agency_UieNumber") || ex.Message.Contains("UK_Agency_EinNumber") || 
                ex.Message.Contains("UK_Agency_SdrNumber"))
            {
                string errorMessage = "El identificador proporcionado ya está registrado en otra agencia.";
                
                if (ex.Message.Contains("UieNumber") || ex.Message.Contains("IUE"))
                {
                    errorMessage = $"El Identificador Único de Entidad (IUE) ya está registrado en otra agencia.";
                }
                else if (ex.Message.Contains("EinNumber") || ex.Message.Contains("EIN"))
                {
                    errorMessage = $"El Número de Seguro Social Patronal (EIN) ya está registrado en otra agencia.";
                }
                else if (ex.Message.Contains("SdrNumber") || ex.Message.Contains("SDR"))
                {
                    errorMessage = $"El Número de Registro del Departamento de Estado (SDR) ya está registrado en otra agencia.";
                }
                
                await _logger.LogError(ex, $"Intento de actualizar agencia {agencyId} con identificador duplicado (constraint): {errorMessage}");
                throw new Exception(errorMessage, ex);
            }
            
            await _logger.LogError(ex, $"Error updating agency {agencyId}: {ex.Message}");
            throw new Exception($"Error al actualizar la agencia {agencyId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Cambia el logo de una agencia
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="imageUrl">Nueva URL de la imagen</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateAgencyLogo(int agencyId, string imageUrl)
    {
        try
        {
            _logger.LogInformation($"Actualizando logo de la agencia {agencyId}");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@imageUrl", imageUrl, DbType.String, ParameterDirection.Input);

            var rowsAffected = await dbConnection.QueryFirstOrDefaultAsync<int>("100_UpdateAgencyLogo", parameters, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error updating agency logo for agency {agencyId}: {ex.Message}");
            throw new Exception($"Error al actualizar el logo de la agencia {agencyId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza el estado de una agencia en el sistema.
    /// Este método permite cambiar el estado actual de una agencia a un nuevo estado,
    /// y opcionalmente proporcionar una justificación en caso de rechazo.
    /// Además, maneja la lógica de notificación por correo electrónico según el nuevo estado.
    /// </summary>
    /// <param name="agencyId">Identificador único de la agencia a actualizar</param>
    /// <param name="statusId">Identificador del nuevo estado a asignar a la agencia</param>
    /// <param name="rejectionJustification">Texto opcional que justifica el rechazo, requerido cuando el estado es de tipo "Rechazado"</param>
    /// <returns>Retorna true si la actualización fue exitosa, false en caso contrario</returns>
    /// <remarks>
    /// Este método realiza las siguientes acciones:
    /// 1. Actualiza el estado de la agencia en la base de datos
    /// 2. Envía notificaciones por correo electrónico según el nuevo estado:
    ///    - En caso de aprobación, envía las credenciales temporales
    ///    - En caso de rechazo, envía la justificación correspondiente
    /// 3. Registra todas las operaciones en el sistema de logging
    /// </remarks>
    public async Task<bool> UpdateAgencyStatus(int agencyId, int statusId, string? rejectionJustification)
    {
        try
        {
            _logger.LogInformation($"Actualizando estado de la agencia {agencyId} a {statusId}");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId);
            parameters.Add("@statusId", statusId);
            parameters.Add("@rejectionJustification", rejectionJustification);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("104_UpdateAgencyStatus", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@ReturnValue");

            if (rowsAffected > 0)
            {
                DTOAgency agency = await GetAgencyById(agencyId);

                if (agency != null)
                {
                    if (statusId == 6) // Rechazo
                    {
                        // Construir el nombre completo del staff
                        var fullName = $"{agency.User?.FirstName} {agency.User?.FatherLastName}".Trim();

                        if (string.IsNullOrEmpty(fullName))
                        {
                            fullName = "Usuario";
                        }

                        await _emailService.SendDenialSponsorEmail(agency.Email, fullName, rejectionJustification ?? "No se proporcionó una justificación");
                        _logger.LogInformation($"Correo de rechazo enviado a la agencia {agencyId}");
                    }
                    else if (statusId == 7) // Aprobado
                    {
                        // Obtener el password temporal usando el UserId del staff
                        var password = await _passwordService.GetTemporaryPassword(agency.User?.UserId); // Use UserId instead of Id

                        // Add a null check before sending the email
                        if (!string.IsNullOrEmpty(password))
                        {
                            // Construir el nombre completo del staff (igual que en el caso de rechazo)
                            var fullName = $"{agency.User?.FirstName} {agency.User?.FatherLastName}".Trim();

                            if (string.IsNullOrEmpty(fullName))
                            {
                                fullName = "Usuario";
                            }

                            // Crear un User temporal para SendApprovalSponsorEmail
                            var User = new User
                            {
                                Id = agency.User?.UserId, // Use UserId (string) instead of Id (int)
                                Email = agency.Email
                            };

                            await _emailService.SendApprovalSponsorEmail(User, password, fullName);
                            _logger.LogInformation($"Correo de aprobación enviado a la agencia {agencyId}");
                        }
                        else
                        {
                            _logger.LogWarning($"No se pudo obtener la contraseña temporal para la usuario {agency.User?.UserId}");
                            // Optionally, you might want to handle this scenario differently
                        }
                    }
                }
            }

            // Invalidar caché
            if (rowsAffected > 0)
            {
                await InvalidateCache(agencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error updating agency status for agency {agencyId} to status {statusId}: {ex.Message}");
            throw new Exception($"Error al actualizar el estado de la agencia {agencyId} al estado {statusId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza el programa de una agencia
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="programId">Id del programa</param>
    /// <param name="userId">Id del usuario</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateAgencyProgram(int agencyId, int programId, string userId)
    {
        try
        {
            _logger.LogInformation($"Actualizando el programa de la agencia {agencyId}");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId);
            parameters.Add("@programId", programId);
            parameters.Add("@userId", userId);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("110_UpdateAgencyProgram", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@ReturnValue");

            if (rowsAffected > 0)
            {
                await InvalidateCache(agencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error updating agency program for agency {agencyId} and program {programId}: {ex.Message}");
            throw new Exception($"Error al actualizar el programa {programId} de la agencia {agencyId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza la inscripción de una agencia desde formulario pre-operacional
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="statusId">Id del estado</param>
    /// <param name="comments">Comentarios</param>
    /// <param name="appointmentCoordinated">Indica si se coordinó la cita</param>
    /// <param name="appointmentDate">Fecha de la cita</param>
    /// <param name="rejectionJustification">Justificación de rechazo</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateAgencyInscription(int agencyId, int statusId, string comments, bool appointmentCoordinated, DateTime? appointmentDate, string? rejectionJustification)
    {
        try
        {
            _logger.LogInformation($"Actualizando la inscripción de la agencia {agencyId}");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId);
            parameters.Add("@statusId", statusId);
            parameters.Add("@comments", comments);
            parameters.Add("@appointmentCoordinated", appointmentCoordinated);
            parameters.Add("@appointmentDate", appointmentDate);
            parameters.Add("@rejectionJustification", rejectionJustification);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("110_UpdateAgencyIncriptionPreOpetational", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                await InvalidateCache(agencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error updating agency inscription for agency {agencyId}: {ex.Message}");
            throw new Exception($"Error al actualizar la inscripción de la agencia {agencyId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Verifica si una agencia es propietaria
    /// </summary>
    /// <param name="agencyId">Id de la agencia a verificar</param>
    /// <returns>True si la agencia es propietaria, False en caso contrario</returns>
    private async Task<bool> IsAgencyPropietary(int agencyId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", agencyId, DbType.Int32, ParameterDirection.Input);

            var result = await dbConnection.QueryMultipleAsync("114_GetAgencyById", parameters, commandType: CommandType.StoredProcedure);
            
            if (result == null)
            {
                return false;
            }

            var agencyDynamic = await result.ReadFirstOrDefaultAsync<dynamic>();
            if (agencyDynamic == null)
            {
                return false;
            }

            // Acceder de forma segura a la propiedad IsPropietary del objeto dinámico
            // Usar conversión a diccionario para acceso seguro
            if (agencyDynamic is IDictionary<string, object> agencyDict && agencyDict.TryGetValue("IsPropietary", out var isPropietaryValue))
            {
                return isPropietaryValue is bool isPropietary && isPropietary;
            }

            // Fallback: intentar acceso directo si la conversión falla
            try
            {
                return agencyDynamic.IsPropietary == true;
            }
            catch
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error verificando si la agencia {agencyId} es propietaria: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Elimina una agencia y sus programas asociados
    /// </summary>
    /// <param name="agencyId">Id de la agencia a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    public async Task<bool> DeleteAgency(int agencyId)
    {
        try
        {
            _logger.LogInformation($"Eliminando la agencia {agencyId}");

            // Validación previa: Verificar si la agencia es propietaria
            if (await IsAgencyPropietary(agencyId))
            {
                var errorMessage = $"No se puede eliminar una agencia propietaria. Esta agencia está protegida y no puede ser eliminada bajo ninguna circunstancia.";
                await _logger.LogError(new Exception(errorMessage), $"Intento de eliminar agencia propietaria {agencyId}");
                throw new Exception(errorMessage);
            }

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_DeleteAgency", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                await InvalidateCache(agencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error deleting agency {agencyId}: {ex.Message}");
            throw new Exception($"Error al eliminar la agencia {agencyId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza la fecha de registro completado de una agencia
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <param name="completedRegistrationDate">Fecha de registro completado</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateCompletedRegistrationDate(int agencyId, DateTime completedRegistrationDate)
    {
        try
        {
            _logger.LogInformation($"Actualizando fecha de registro completado para la agencia {agencyId}");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId);
            parameters.Add("@completedRegistrationDate", completedRegistrationDate);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("110_UpdateCompletedRegistrationDate", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                await InvalidateCache(agencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error updating completed registration date for agency {agencyId}: {ex.Message}");
            throw new Exception($"Error al actualizar la fecha de registro completado de la agencia {agencyId}: {ex.Message}", ex);
        }
    }

    private async Task InvalidateCache(int? agencyId = null)
    {
        try
        {
            _logger.LogInformation("Iniciando invalidación de caché");

            // Obtener todas las claves actuales en caché
            var allKeys = _cache.GetKeys<string>().ToList();
            _logger.LogInformation($"Total de claves en caché antes de invalidar: {allKeys.Count}");

            if (agencyId.HasValue)
            {
                // Invalidar caché específico de la agencia
                var agencyKey = string.Format(_appSettings.Cache.Keys.Agency, agencyId);
                _logger.LogInformation($"Invalidando caché de agencia con clave: {agencyKey}");
                _cache.Remove(agencyKey);

                // Invalidar caché de la agencia con usuario
                var userPattern = $"Agency_{agencyId}_User";
                _logger.LogInformation($"Invalidando caché de usuarios de agencia con patrón: {userPattern}", new Dictionary<string, string> { { "Pattern", userPattern } });
                _cache.RemoveByPattern(userPattern);
            }

            // Invalidar listas completas
            _logger.LogInformation("Invalidando lista completa de agencias");
            _cache.Remove(_appSettings.Cache.Keys.Agencies);

            // Invalidar caché de programas de agencia
            var programPattern = "Agency_Programs";
            _logger.LogInformation($"Invalidando caché de programas con patrón: {programPattern}", new Dictionary<string, string> { { "Pattern", programPattern } });
            _cache.RemoveByPattern(programPattern);

            // Verificar claves restantes
            var remainingKeys = _cache.GetKeys<string>().ToList();
            _logger.LogInformation($"Total de claves en caché después de invalidar: {remainingKeys.Count}");
            _logger.LogInformation($"Claves restantes: {string.Join(", ", remainingKeys)}");

            _logger.LogInformation("Cache invalidado para Agency Repository");
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error invalidating cache for Agency Repository: {ex.Message}");
            // No relanzamos la excepción para evitar que un error de caché afecte la operación principal
        }
    }

    /// <summary>
    /// Obtiene el UserId del evaluador (monitor) asignado a una agencia
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>UserId del evaluador o null si no se encuentra</returns>
    public async Task<string?> GetEvaluatorUserIdByAgencyId(int agencyId)
    {
        try
        {
            // Intentar obtener desde el objeto Monitor primero
            var agency = await GetAgencyById(agencyId);
            if (agency != null)
            {
                var monitor = agency.Monitor;
                if (monitor != null)
                {
                    var userId = monitor.UserId?.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        _logger.LogInformation($"EvaluatorUserId obtenido desde Monitor para agencia {agencyId}: {userId}");
                        return userId;
                    }
                }
            }

            // Si no se encontró, consultar directamente AgencyUsers
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId, DbType.Int32);

            var evaluatorUserId = await dbConnection.QueryFirstOrDefaultAsync<string>(
                "SELECT TOP 1 UserId FROM AgencyUsers WHERE AgencyId = @agencyId AND AgencyAssignmentType LIKE 'NUTRE_%' AND IsActive = 1",
                parameters
            );

            if (!string.IsNullOrEmpty(evaluatorUserId))
            {
                _logger.LogInformation($"EvaluatorUserId obtenido desde AgencyUsers para agencia {agencyId}: {evaluatorUserId}");
            }
            else
            {
                _logger.LogWarning($"No se encontró evaluador asignado para la agencia {agencyId}");
            }

            return evaluatorUserId;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error obteniendo UserId del monitor desde AgencyUsers para agencia {agencyId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Obtiene todos los UserIds de evaluadores relacionados a una agencia
    /// Incluye evaluadores asignados directamente a la agencia y evaluadores asignados a los programas de la agencia
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>Lista de UserIds únicos de evaluadores</returns>
    public async Task<List<string>> GetAllEvaluatorUserIdsByAgencyId(int agencyId)
    {
        try
        {
            var evaluatorUserIds = new HashSet<string>();

            using IDbConnection dbConnection = _context.CreateConnection();

            // 1. Obtener evaluadores asignados directamente a la agencia
            // Desde AgencyUsers con AgencyAssignmentType LIKE 'NUTRE_%' y rol "Evaluador"
            var agencyEvaluatorsQuery = @"
                SELECT DISTINCT au.UserId
                FROM AgencyUsers au
                    INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
                    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE au.AgencyId = @agencyId
                    AND au.AgencyAssignmentType LIKE 'NUTRE_%'
                    AND au.IsActive = 1
                    AND r.Name = 'Evaluador'";

            var agencyEvaluators = await dbConnection.QueryAsync<string>(
                agencyEvaluatorsQuery,
                new { agencyId }
            );

            foreach (var userId in agencyEvaluators)
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    evaluatorUserIds.Add(userId);
                }
            }

            _logger.LogInformation($"Se encontraron {evaluatorUserIds.Count} evaluadores asignados directamente a la agencia {agencyId}");

            // 2. Obtener todos los programas de la agencia
            var programsQuery = @"
                SELECT DISTINCT ProgramId
                FROM AgencyProgram
                WHERE AgencyId = @agencyId
                    AND IsActive = 1";

            var programIds = await dbConnection.QueryAsync<int>(programsQuery, new { agencyId });
            var programIdList = programIds.ToList();

            _logger.LogInformation($"La agencia {agencyId} tiene {programIdList.Count} programas activos");

            // 3. Para cada programa, obtener evaluadores asignados
            // Desde UserProgram con IsActive = 1 y rol "Evaluador"
            if (programIdList.Any())
            {
                var programEvaluatorsQuery = @"
                    SELECT DISTINCT up.UserId
                    FROM UserProgram up
                        INNER JOIN AspNetUsers u ON up.UserId = u.Id
                        INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                    WHERE up.ProgramId IN @programIds
                        AND up.IsActive = 1
                        AND r.Name = 'Evaluador'";

                var programEvaluators = await dbConnection.QueryAsync<string>(
                    programEvaluatorsQuery,
                    new { programIds = programIdList }
                );

                foreach (var userId in programEvaluators)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        evaluatorUserIds.Add(userId);
                    }
                }

                _logger.LogInformation($"Se encontraron {evaluatorUserIds.Count} evaluadores totales (agencia + programas) para la agencia {agencyId}");
            }

            var result = evaluatorUserIds.ToList();
            _logger.LogInformation($"Total de evaluadores únicos para la agencia {agencyId}: {result.Count}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error obteniendo todos los evaluadores para la agencia {agencyId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Verifica si un IUE (Identificador Único de Entidad) ya existe en la tabla Agency
    /// </summary>
    /// <param name="uieNumber">El número IUE a verificar</param>
    /// <returns>True si el IUE existe, False si no existe</returns>
    public async Task<bool> UieNumberExists(long uieNumber)
    {
        try
        {
            // Log para debugging
            _logger.LogInformation($"UieNumberExists - Verificando IUE: {uieNumber} (tipo: {uieNumber.GetType().Name})", new Dictionary<string, string> 
            { 
                { "UieNumber", uieNumber.ToString() },
                { "Type", uieNumber.GetType().Name }
            });

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@uieNumber", uieNumber, DbType.Int64);
            parameters.Add("@exists", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_CheckUieNumberExists", parameters, commandType: CommandType.StoredProcedure);

            var exists = parameters.Get<bool>("@exists");
            
            // Log del resultado
            _logger.LogInformation($"UieNumberExists - Resultado para IUE {uieNumber}: {exists}", new Dictionary<string, string> 
            { 
                { "UieNumber", uieNumber.ToString() },
                { "Exists", exists.ToString() }
            });

            return exists;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error al verificar si el IUE existe: {ex.Message}");
            // En caso de error, retornar false para no bloquear el registro
            return false;
        }
    }

    /// <summary>
    /// Verifica si un SDR (Número de Registro del Departamento de Estado) ya existe en la tabla Agency
    /// </summary>
    /// <param name="sdrNumber">El número SDR a verificar</param>
    /// <returns>True si el SDR existe, False si no existe</returns>
    public async Task<bool> SdrNumberExists(long sdrNumber)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@sdrNumber", sdrNumber, DbType.Int64);
            parameters.Add("@exists", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_CheckSdrNumberExists", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@exists");
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error al verificar si el SDR existe: {ex.Message}");
            // En caso de error, retornar false para no bloquear el registro
            return false;
        }
    }

    /// <summary>
    /// Verifica si un EIN (Número de Seguro Social Patronal) ya existe en la tabla Agency
    /// </summary>
    /// <param name="einNumber">El número EIN a verificar</param>
    /// <returns>True si el EIN existe, False si no existe</returns>
    public async Task<bool> EinNumberExists(int einNumber)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@einNumber", einNumber, DbType.Int32);
            parameters.Add("@exists", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_CheckEinNumberExists", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@exists");
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, $"Error al verificar si el EIN existe: {ex.Message}");
            // En caso de error, retornar false para no bloquear el registro
            return false;
        }
    }

}
