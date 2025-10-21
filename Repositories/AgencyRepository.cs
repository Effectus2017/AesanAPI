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

            var result = await dbConnection.QueryMultipleAsync("113_GetAgencyById", parameters, commandType: CommandType.StoredProcedure);

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
            var result = await dbConnection.QueryMultipleAsync("112_GetAgencyByIdAndUserId", parameters, commandType: CommandType.StoredProcedure);

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
    /// <param name="take">El número de agencias a obtener</param>
    /// <param name="skip">El número de agencias a saltar</param>
    /// <param name="name">El nombre de la agencia</param>
    /// <param name="alls">Si se deben obtener todas las agencias</param>
    /// <returns>Las agencias</returns>
    public async Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, string? userId, bool alls, bool isList, bool? isPropietary)
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

            if (isList)
            {
                using var result = await dbConnection.QueryMultipleAsync("117_GetAgencies", param, commandType: CommandType.StoredProcedure);

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

                using var result = await dbConnection.QueryMultipleAsync("117_GetAgencies", param, commandType: CommandType.StoredProcedure);

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
            await InsertAgencyInscription(
                agencyId,
                agencyRequest.NonProfit,
                agencyRequest.FederalFundsDenied,
                agencyRequest.FederalFundsDeniedReason, // Nuevo argumento
                agencyRequest.StateFundsDenied,
                agencyRequest.StateFundsDeniedReason,
                agencyRequest.BasicEducationRegistry,
                agencyRequest.ExtendedHours,
                agencyRequest.TaxExemptionStatusId,
                agencyRequest.TaxExemptionTypeId,
                agencyRequest.PublicAllianceContractId,
                agencyRequest.NationalYouthProgram,
                agencyRequest.IsDayCareHome
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
    public async Task<int> InsertAgencyInscription(int agencyId, bool nonProfit, bool federalFundsDenied, string? federalFundsDeniedReason, bool stateFundsDenied, string? stateFundsDeniedReason, bool basicEducationRegistry, bool extendedHours, int taxExemptionStatusId, int taxExemptionTypeId, int publicAllianceContractId, bool nationalYouthProgram, bool isDayCareHome)
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
            parameters.Add("@taxExemptionStatusId", taxExemptionStatusId);
            parameters.Add("@taxExemptionTypeId", taxExemptionTypeId);
            parameters.Add("@publicAllianceContractId", publicAllianceContractId);
            parameters.Add("@nationalYouthProgram", nationalYouthProgram);
            parameters.Add("@isDayCareHome", isDayCareHome);

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
                await _agencyUsersRepository.AssignAgencyToUser(agencyRequest.MonitorId.ToString(), agencyId, agencyRequest.AssignedBy, false, true);
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
                            // Crear un User temporal para SendApprovalSponsorEmail
                            var User = new User
                            {
                                Id = agency.User?.UserId, // Use UserId (string) instead of Id (int)
                                Email = agency.Email
                            };

                            await _emailService.SendApprovalSponsorEmail(User, password);
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

            await dbConnection.ExecuteAsync("114_UpdateCompletedRegistrationDate", parameters, commandType: CommandType.StoredProcedure);
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


}
