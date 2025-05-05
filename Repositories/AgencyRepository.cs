using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
namespace Api.Repositories;

public class AgencyRepository(
    IEmailService emailService,
    IPasswordService passwordService,
    IAgencyUsersRepository agencyUsersRepository,
    DapperContext context,
    ILoggingService loggingService,
    IMemoryCache cache,
    IOptions<ApplicationSettings> appSettings) : IAgencyRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly IPasswordService _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
    private readonly IAgencyUsersRepository _agencyUsersRepository = agencyUsersRepository ?? throw new ArgumentNullException(nameof(agencyUsersRepository));
    private readonly IMemoryCache _cache = cache;
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
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

            var result = await dbConnection.QueryMultipleAsync("110_GetAgencyById", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                var error = new Exception($"No se encontraron resultados para la agencia ID: {id}");
                await _logger.LogError(error, "No se encontraron resultados");
                return null;
            }

            var _agencyResult = await result.ReadFirstOrDefaultAsync<dynamic>();

            if (_agencyResult == null)
            {
                var error = new Exception($"No se encontró la agencia con ID: {id}");
                await _logger.LogError(error, "No se encontró la agencia");
                return null;
            }

            var agency = MapAgencyFromResult(_agencyResult);
            var _agenciesPrograms = await result.ReadAsync<dynamic>();

            if (_agenciesPrograms.Any())
            {
                agency.Programs = MapProgramsFromResult(_agenciesPrograms);
            }

            return agency;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw;
        }
    }

    /// <summary>
    /// Obtiene una agencia por su ID
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    public async Task<dynamic> GetAgencyByIdAndUserId(int agencyId, string userId)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.Agency + "_User_{0}_{1}", agencyId, userId);
        _logger.LogInformation($"Obteniendo agencia del caché con clave: {cacheKey}");

        return await _cache.CacheAgencyQuery(
            cacheKey,
            async () =>
            {
                _logger.LogInformation($"Caché no encontrado, obteniendo datos de la base de datos para agencia {agencyId} y usuario {userId}");
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@agencyId", agencyId);
                parameters.Add("@userId", userId);
                var result = await dbConnection.QueryMultipleAsync("110_GetAgencyByIdAndUserId", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var _agencyResult = await result.ReadFirstOrDefaultAsync<dynamic>();

                if (_agencyResult == null)
                {
                    return null;
                }

                var agency = MapAgencyFromResult(_agencyResult);
                var _agenciesPrograms = await result.ReadAsync<dynamic>();

                if (_agenciesPrograms != null && _agenciesPrograms.Any())
                {
                    agency.Programs = MapProgramsFromResult(_agenciesPrograms);
                }

                _logger.LogInformation($"Datos obtenidos de la base de datos y almacenados en caché con clave: {cacheKey}");
                return agency!;
            },
            loggingService,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene todas las agencias de la base de datos
    /// </summary>
    /// <param name="take">El número de agencias a obtener</param>
    /// <param name="skip">El número de agencias a saltar</param>
    /// <param name="name">El nombre de la agencia</param>
    /// <param name="alls">Si se deben obtener todas las agencias</param>
    /// <returns>Las agencias</returns>
    public async Task<dynamic> GetAllAgenciesFromDbOld(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, string? userId, bool alls)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.Agencies, take, skip, name ?? string.Empty, userId ?? string.Empty, alls);

        return await _cache.CacheAgencyQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@take", take);
                parameters.Add("@skip", skip);
                parameters.Add("@name", name);
                parameters.Add("@regionId", regionId);
                parameters.Add("@cityId", cityId);
                parameters.Add("@programId", programId);
                parameters.Add("@statusId", statusId);
                parameters.Add("@userId", userId);
                parameters.Add("@alls", alls);

                // Variables para almacenar los resultados
                List<dynamic> agencies = new List<dynamic>();
                List<dynamic> agenciesPrograms = new List<dynamic>();
                int count = 0;

                // Usar un bloque using para garantizar que el GridReader se cierre correctamente
                using (var result = await dbConnection.QueryMultipleAsync("114_GetAgencies", parameters, commandType: CommandType.StoredProcedure))
                {
                    if (result == null)
                    {
                        return null;
                    }

                    // Leer todos los conjuntos de resultados de manera segura
                    if (!result.IsConsumed)
                    {
                        agencies = result.Read<dynamic>().ToList();
                    }

                    if (!result.IsConsumed)
                    {
                        agenciesPrograms = result.Read<dynamic>().ToList();
                    }

                    if (!result.IsConsumed)
                    {
                        count = result.Read<int>().FirstOrDefault();
                    }
                } // El GridReader se cierra aquí

                // Procesar los datos después de que el GridReader se haya cerrado
                var mappedAgencies = agencies.Select(MapAgencyFromResult).ToList();

                if (agenciesPrograms != null && agenciesPrograms.Any())
                {
                    foreach (var agency in mappedAgencies)
                    {
                        agency.Programs = MapProgramsFromResult(
                            agenciesPrograms.Where(ap => ap.AgencyId == agency.Id)
                        );
                    }
                }

                return new { data = mappedAgencies, count };
            },
            loggingService,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene todas las agencias de la base de datos
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <param name="name">El nombre de la agencia</param>
    /// <param name="alls">Si se deben obtener todas las agencias</param>
    /// <returns>Las agencias</returns>
    public async Task<dynamic> GetAllAgenciesList(int? id, string name, bool alls)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id);
            parameters.Add("@name", name);
            parameters.Add("@alls", alls);

            var result = await dbConnection.QueryMultipleAsync("101_GetAgenciesList", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
                return null;

            var _agenciesResult = await result.ReadAsync<dynamic>();

            if (_agenciesResult == null)
                return null;

            return _agenciesResult;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener las agencias", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw;
        }
    }

    /// <summary>
    /// Obtiene los programas de una agencia por el ID del usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>Los programas de la agencia</returns>
    public async Task<dynamic> GetAgencyProgramsByUserId(string userId)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.Agency + "_Programs_{0}", userId);

        return await _cache.CacheAgencyQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId);
                var result = await dbConnection.QueryAsync<DTOProgram>("100_GetAgencyProgramsByUserId", parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            },
            loggingService,
            _appSettings
        );
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
            parameters.Add("@Name", agencyRequest.Name);
            parameters.Add("@AgencyStatusId", agencyRequest.StatusId);
            parameters.Add("@SdrNumber", agencyRequest.SdrNumber);
            parameters.Add("@UieNumber", agencyRequest.UieNumber);
            parameters.Add("@EinNumber", agencyRequest.EinNumber);
            parameters.Add("@Address", agencyRequest.Address);
            parameters.Add("@ZipCode", agencyRequest.ZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@Phone", agencyRequest.Phone);
            parameters.Add("@Email", agencyRequest.Email);
            parameters.Add("@CityId", agencyRequest.CityId);
            parameters.Add("@RegionId", agencyRequest.RegionId);
            parameters.Add("@PostalAddress", agencyRequest.PostalAddress);
            parameters.Add("@PostalZipCode", agencyRequest.PostalZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@PostalCityId", agencyRequest.PostalCityId);
            parameters.Add("@PostalRegionId", agencyRequest.PostalRegionId);
            parameters.Add("@Latitude", Math.Round(agencyRequest.Latitude, 2));
            parameters.Add("@Longitude", Math.Round(agencyRequest.Longitude, 2));
            parameters.Add("@ImageURL", agencyRequest.ImageUrl);
            parameters.Add("@IsActive", agencyRequest.IsActive);
            parameters.Add("@IsPropietary", true);
            parameters.Add("@IsListable", agencyRequest.IsListable);
            parameters.Add("@AgencyCode", agencyRequest.AgencyCode);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("111_InsertAgency", parameters, commandType: CommandType.StoredProcedure);

            var agencyId = parameters.Get<int>("@Id");

            if (agencyId <= 0)
            {
                throw new Exception("Error al insertar la agencia");
            }

            // Insertar la solicitud de participación de la Agencia
            await InsertAgencyInscription(
                agencyId,
                agencyRequest.NonProfit,
                agencyRequest.FederalFundsDenied,
                agencyRequest.StateFundsDenied,
                agencyRequest.OrganizedAthleticPrograms,
                agencyRequest.AtRiskService,
                agencyRequest.BasicEducationRegistry,
                agencyRequest.ServiceTime,
                agencyRequest.TaxExemptionStatus,
                agencyRequest.TaxExemptionType
            );

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
            await _logger.LogError(ex, "Error al insertar la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw;
        }
    }

    /// <summary>
    /// Inserta una inscripción de agencia en la base de datos
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="nonProfit">Si es sin fines de lucro</param>
    /// <param name="federalFundsDenied">Fondos federales denegados</param>
    /// <param name="stateFundsDenied">Fondos estatales denegados</param>
    /// <param name="organizedAthleticPrograms">Programas atléticos organizados</param>
    /// <param name="atRiskService">Servicio a personas en riesgo</param>
    /// <param name="basicEducationRegistry">Registro de educación básica</param>
    /// <param name="serviceTime">Tiempo de servicio</param>
    /// <param name="taxExemptionStatus">Estado de exención de impuestos</param>
    /// <param name="taxExemptionType">Tipo de exención de impuestos</param>
    /// <returns>El Id de la inscripción insertada</returns>
    public async Task<int> InsertAgencyInscription(int agencyId, bool nonProfit, bool federalFundsDenied, bool stateFundsDenied, bool organizedAthleticPrograms, bool atRiskService, int basicEducationRegistry, DateTime serviceTime, int taxExemptionStatus, int taxExemptionType)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AgencyId", agencyId);
            parameters.Add("@NonProfit", nonProfit);
            parameters.Add("@FederalFundsDenied", federalFundsDenied);
            parameters.Add("@StateFundsDenied", stateFundsDenied);
            parameters.Add("@OrganizedAthleticPrograms", organizedAthleticPrograms);
            parameters.Add("@AtRiskService", atRiskService);
            parameters.Add("@BasicEducationRegistry", basicEducationRegistry);
            parameters.Add("@ServiceTime", serviceTime);
            parameters.Add("@TaxExemptionStatus", taxExemptionStatus);
            parameters.Add("@TaxExemptionType", taxExemptionType);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("111_InsertAgencyInscription", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@Id");
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al insertar la inscripción de la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw;
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
            await _logger.LogError(ex, "Error al insertar el programa de la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw new Exception(ex.Message);
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
            // Obtener la agencia actual para comparar el monitor
            var currentAgency = await GetAgencyById(agencyId);

            if (currentAgency == null)
            {
                throw new ArgumentNullException(nameof(currentAgency), "La agencia actual no puede ser nula");
            }

            string currentMonitorId = currentAgency?.Monitor?.Id;

            var parameters = new DynamicParameters();
            parameters.Add("@Id", agencyId);
            parameters.Add("@Name", agencyRequest.Name);
            parameters.Add("@AgencyStatusId", agencyRequest.StatusId);
            parameters.Add("@SdrNumber", agencyRequest.SdrNumber);
            parameters.Add("@UieNumber", agencyRequest.UieNumber);
            parameters.Add("@EinNumber", agencyRequest.EinNumber);
            parameters.Add("@Address", agencyRequest.Address);
            parameters.Add("@ZipCode", agencyRequest.ZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@CityId", agencyRequest.CityId);
            parameters.Add("@RegionId", agencyRequest.RegionId);
            parameters.Add("@Latitude", Math.Round(agencyRequest.Latitude, 2));
            parameters.Add("@Longitude", Math.Round(agencyRequest.Longitude, 2));
            parameters.Add("@PostalAddress", agencyRequest.PostalAddress);
            parameters.Add("@PostalZipCode", agencyRequest.PostalZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@PostalCityId", agencyRequest.PostalCityId);
            parameters.Add("@PostalRegionId", agencyRequest.PostalRegionId);
            parameters.Add("@Phone", agencyRequest.Phone);
            parameters.Add("@ImageURL", agencyRequest.ImageUrl);
            parameters.Add("@Email", agencyRequest.Email);

            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("110_UpdateAgency", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            // Verificar si hay un nuevo monitor asignado y es diferente al actual
            if (!string.IsNullOrEmpty(agencyRequest.MonitorId) && currentMonitorId != agencyRequest.MonitorId)
            {
                // Si había un monitor previo, des asignar el monitor primero
                if (!string.IsNullOrEmpty(currentMonitorId))
                {
                    _logger.LogInformation($"Des asignando monitor anterior {currentMonitorId} de la agencia {agencyId}");
                    await _agencyUsersRepository.UnassignAgencyFromUser(currentMonitorId, agencyId);

                    // Enviar correo de des asignación al monitor anterior
                    var previousMonitor = new DTOUser
                    {
                        Id = currentMonitorId,
                        FirstName = currentAgency.Monitor.FirstName,
                        FatherLastName = currentAgency.Monitor.FatherLastName,
                        Email = currentAgency.Monitor.Email
                    };
                    await _emailService.SendAgencyUnassignmentEmail(previousMonitor, currentAgency);
                }

                // Asignar el nuevo monitor
                _logger.LogInformation($"Asignando nuevo monitor {agencyRequest.MonitorId} a la agencia {agencyId}");
                await _agencyUsersRepository.AssignAgencyToUser(agencyRequest.MonitorId, agencyId, agencyRequest.AssignedBy, false, true);
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
            await _logger.LogError(ex, "Error al actualizar la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw;
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
            parameters.Add("@AgencyId", agencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ImageUrl", imageUrl, DbType.String, ParameterDirection.Input);

            var rowsAffected = await dbConnection.QueryFirstOrDefaultAsync<int>("100_UpdateAgencyLogo", parameters, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al actualizar el logo de la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw new Exception(ex.Message);
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
                    var User = new User
                    {
                        Id = agency.User.Id,
                        FirstName = agency.User.FirstName,
                        FatherLastName = agency.User.FatherLastName,
                        Email = agency.Email
                    };

                    if (statusId == 6) // Rechazo
                    {
                        await _emailService.SendDenialSponsorEmail(User, rejectionJustification ?? "No se proporcionó una justificación");
                        _logger.LogInformation($"Correo de rechazo enviado a la agencia {agencyId}");
                    }
                    else if (statusId == 7) // Aprobado
                    {
                        // Obtener el password temporal
                        var password = await _passwordService.GetTemporaryPassword(User.Id); // Obtain the temporary password

                        // Add a null check before sending the email
                        if (!string.IsNullOrEmpty(password))
                        {
                            await _emailService.SendApprovalSponsorEmail(User, password);
                            _logger.LogInformation($"Correo de aprobación enviado a la agencia {agencyId}");
                        }
                        else
                        {
                            _logger.LogWarning($"No se pudo obtener la contraseña temporal para la usuario {User.Id}");
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
            await _logger.LogError(ex, "Error al actualizar el estado de la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw;
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
            await _logger.LogError(ex, "Error al actualizar el programa de la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw;
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
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateAgencyInscription(int agencyId, int statusId, string comments, bool appointmentCoordinated, DateTime? appointmentDate)
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
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("110_UpdateAgencyIncriptionPreOpetational", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@ReturnValue");

            if (rowsAffected > 0)
            {
                await InvalidateCache(agencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al actualizar la inscripción de la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw;
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

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new { agencyId };

            // Llamar al procedimiento almacenado para eliminar la agencia y sus programas
            var rowsAffected = await dbConnection.QueryFirstOrDefaultAsync<int>("100_DeleteAgency", param, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                // Invalidar caché
                await InvalidateCache(agencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al eliminar la agencia", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw new Exception(ex.Message);
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
            await _logger.LogError(ex, "Error al invalidar el caché de Agency Repository", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            // No relanzamos la excepción para evitar que un error de caché afecte la operación principal
        }
    }

    /// <summary>
    /// Mapea una agencia desde un resultado dinámico a un DTOAgency
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAgency</returns>
    private static DTOAgency MapAgencyFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOAgency
            {
                Id = item.Id ?? 0,
                Name = item.Name ?? string.Empty,
                StatusId = item.AgencyStatusId ?? 0,
                SdrNumber = item.SdrNumber,
                UieNumber = item.UieNumber,
                EinNumber = item.EinNumber,
                Address = item.Address,
                ZipCode = item.ZipCode,
                PostalAddress = item.PostalAddress,
                PostalZipCode = item.PostalZipCode,
                Latitude = item.Latitude != null ? (float)item.Latitude : null,
                Longitude = item.Longitude != null ? (float)item.Longitude : null,
                Phone = item.Phone,
                Email = item.Email,
                ImageURL = item.ImageURL,

                BasicEducationRegistry = item.BasicEducationRegistry ?? 0,

                // Justificación de rechazo
                RejectionJustification = item.RejectionJustification,
                // Comentarios
                Comments = item.Comments,
                // Cita coordinada
                AppointmentCoordinated = item.AppointmentCoordinated ?? false,
                // Fecha de la cita
                AppointmentDate = item.AppointmentDate,

                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                AgencyCode = item.AgencyCode,

                City = new DTOCity
                {
                    Id = item.CityId ?? 0,
                    Name = item.CityName ?? string.Empty
                },
                Region = new DTORegion
                {
                    Id = item.RegionId ?? 0,
                    Name = item.RegionName ?? string.Empty
                },
                PostalCity = new DTOCity
                {
                    Id = item.PostalCityId ?? 0,
                    Name = item.PostalCityName ?? string.Empty
                },
                PostalRegion = new DTORegion
                {
                    Id = item.PostalRegionId ?? 0,
                    Name = item.PostalRegionName ?? string.Empty
                },
                Status = new DTOAgencyStatus
                {
                    Id = item.AgencyStatusId ?? 0,
                    Name = item.AgencyStatusName ?? string.Empty
                },
                User = item.UserId != null ? new DTOUser
                {
                    Id = item.UserId,
                    FirstName = item.UserFirstName ?? string.Empty,
                    MiddleName = item.UserMiddleName ?? string.Empty,
                    FatherLastName = item.UserFatherLastName ?? string.Empty,
                    MotherLastName = item.UserMotherLastName ?? string.Empty,
                    AdministrationTitle = item.UserAdministrationTitle ?? string.Empty,
                    Email = item.UserEmail ?? string.Empty,
                    Phone = item.UserPhone ?? string.Empty,
                    ImageURL = item.UserImageURL ?? string.Empty,
                } : null,
                Monitor = item.MonitorId != null ? new DTOUser
                {
                    Id = item.MonitorId,
                    FirstName = item.MonitorFirstName ?? string.Empty,
                    FatherLastName = item.MonitorFatherLastName ?? string.Empty,
                } : null
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la agencia: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la agencia: {ex.Message}", ex);
        }
    }

    private static List<DTOProgram> MapProgramsFromResult(IEnumerable<dynamic> programs)
    {
        try
        {
            return programs
                .Select(
                    ap =>
                        new DTOProgram
                        {
                            Id = ap.Id,
                            Name = ap.Name,
                            Description = ap.Description,
                            AgencyId = ap.AgencyId
                        }
                )
                .ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear los programas: {ex.Message}", ex);
        }
    }

    public async Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, string? userId, bool alls)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@take", take);
            param.Add("@skip", skip);
            param.Add("@name", name);
            param.Add("@regionId", regionId);
            param.Add("@cityId", cityId);
            param.Add("@programId", programId);
            param.Add("@statusId", statusId);
            param.Add("@userId", userId);
            param.Add("@alls", alls);

            // Variables para almacenar los resultados
            List<dynamic> agencies = new List<dynamic>();
            List<dynamic> agenciesPrograms = new List<dynamic>();
            int count = 0;

            // Usar un bloque using para garantizar que el GridReader se cierre correctamente
            using (var result = await dbConnection.QueryMultipleAsync(
                "114_GetAgencies",
                param,
                commandType: CommandType.StoredProcedure))
            {
                if (result == null)
                {
                    return null;
                }

                // Leer todos los conjuntos de resultados de manera segura
                if (!result.IsConsumed)
                {
                    agencies = result.Read<dynamic>().ToList();
                }

                if (!result.IsConsumed)
                {
                    agenciesPrograms = result.Read<dynamic>().ToList();
                }

                if (!result.IsConsumed)
                {
                    count = result.Read<int>().FirstOrDefault();
                }
            } // El GridReader se cierra aquí

            // Procesar los datos después de que el GridReader se haya cerrado
            var mappedAgencies = agencies.Select(MapAgencyFromResult).ToList();

            if (agenciesPrograms != null && agenciesPrograms.Any())
            {
                foreach (var agency in mappedAgencies)
                {
                    agency.Programs = MapProgramsFromResult(
                        agenciesPrograms.Where(ap => ap.AgencyId == agency.Id)
                    );
                }
            }

            return new { data = mappedAgencies, count };
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener las agencias de la base de datos", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });
            throw;
        }
    }
}