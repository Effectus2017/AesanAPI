using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Repositories;

public class AgencyRepository(
    IEmailService emailService,
    IPasswordService passwordService,
    IAgencyUserAssignmentRepository agencyUserAssignmentRepository,
    DapperContext context,
    ILogger<AgencyRepository> logger,
    IMemoryCache cache,
    ApplicationSettings appSettings) : IAgencyRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AgencyRepository> _logger = logger;
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly IPasswordService _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
    private readonly IAgencyUserAssignmentRepository _agencyUserAssignmentRepository = agencyUserAssignmentRepository ?? throw new ArgumentNullException(nameof(agencyUserAssignmentRepository));
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings;

    /// <summary>
    /// Obtiene una agencia por su ID
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    public async Task<dynamic> GetAgencyById(int id)
    {
        using IDbConnection dbConnection = _context.CreateConnection();

        var param = new DynamicParameters();
        param.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

        var result = await dbConnection.QueryMultipleAsync(
            "102_GetAgencyById",
            param,
            commandType: CommandType.StoredProcedure
        );

        if (result == null)
            return null;

        var _agencyResult = await result.ReadFirstOrDefaultAsync<dynamic>();

        if (_agencyResult == null)
            return null;

        var agency = MapAgencyFromResult(_agencyResult);
        var _agenciesPrograms = await result.ReadAsync<dynamic>();

        if (_agenciesPrograms.Any())
        {
            agency.Programs = MapProgramsFromResult(_agenciesPrograms);
        }

        return agency;
    }

    /// <summary>
    /// Obtiene una agencia por su ID
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    public async Task<dynamic> GetAgencyByIdAndUserId(int agencyId, string userId)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.Agency + "_User_{1}", agencyId, userId);

        return await _cache.CacheAgencyQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var param = new { agencyId, userId };
                var result = await dbConnection.QueryMultipleAsync(
                    "101_GetAgencyByIdAndUserId",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                    return null;

                var _agencyResult = await result.ReadFirstOrDefaultAsync<dynamic>();

                if (_agencyResult == null)
                    return null;

                var agency = MapAgencyFromResult(_agencyResult);
                var _agenciesPrograms = await result.ReadAsync<dynamic>();

                if (_agenciesPrograms != null && _agenciesPrograms.Any())
                {
                    agency.Programs = MapProgramsFromResult(_agenciesPrograms);
                }

                return agency!;
            },
            _logger,
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
    public async Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, string? userId, bool alls)
    {
        string cacheKey = string.Format(
            _appSettings.Cache.Keys.Agencies,
            take,
            skip,
            name ?? string.Empty,
            userId ?? string.Empty,
            alls
        );

        return await _cache.CacheAgencyQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var param = new DynamicParameters();
                param.Add("@take", take);
                param.Add("@skip", skip);
                param.Add("@name", name);
                param.Add("@regionId", regionId);
                param.Add("@cityId", cityId);
                param.Add("@programId", programId);
                param.Add("@alls", alls);

                // Datos del usuario monitor
                param.Add("@userId", userId);

                var result = await dbConnection.QueryMultipleAsync(
                    "104_GetAgencies",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    return null;
                }

                var agencies = result.Read<dynamic>().Select(MapAgencyFromResult).ToList();
                var _agenciesPrograms = await result.ReadAsync<dynamic>();

                if (_agenciesPrograms != null && _agenciesPrograms.Any())
                {
                    foreach (var agency in agencies)
                    {
                        agency.Programs = MapProgramsFromResult(
                            _agenciesPrograms.Where(ap => ap.AgencyId == agency.Id)
                        );
                    }
                }

                var count = result.Read<int>().Single();
                return new { data = agencies, count };
            },
            _logger,
            _appSettings
        );
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
                var param = new { userId };
                var result = await dbConnection.QueryAsync<DTOProgram>(
                    "100_GetAgencyProgramsByUserId",
                    param,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            },
            _logger,
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
            parameters.Add("@ZipCode", agencyRequest.ZipCode);
            parameters.Add("@Phone", agencyRequest.Phone);
            parameters.Add("@Email", agencyRequest.Email);
            parameters.Add("@CityId", agencyRequest.CityId);
            parameters.Add("@RegionId", agencyRequest.RegionId);
            parameters.Add("@PostalAddress", agencyRequest.PostalAddress);
            parameters.Add("@PostalZipCode", agencyRequest.PostalZipCode);
            parameters.Add("@PostalCityId", agencyRequest.PostalCityId);
            parameters.Add("@PostalRegionId", agencyRequest.PostalRegionId);
            parameters.Add("@Latitude", agencyRequest.Latitude);
            parameters.Add("@Longitude", agencyRequest.Longitude);
            parameters.Add("@NonProfit", agencyRequest.NonProfit);
            parameters.Add("@FederalFundsDenied", agencyRequest.FederalFundsDenied);
            parameters.Add("@StateFundsDenied", agencyRequest.StateFundsDenied);
            parameters.Add("@OrganizedAthleticPrograms", agencyRequest.OrganizedAthleticPrograms);
            parameters.Add("@AtRiskService", agencyRequest.AtRiskService);
            parameters.Add("@ServiceTime", agencyRequest.ServiceTime);
            parameters.Add("@TaxExemptionStatus", agencyRequest.TaxExemptionStatus);
            parameters.Add("@TaxExemptionType", agencyRequest.TaxExemptionType);
            parameters.Add("@ImageURL", agencyRequest.ImageUrl);
            parameters.Add("@IsListable", agencyRequest.IsListable);
            parameters.Add("@AgencyCode", agencyRequest.AgencyCode);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("103_InsertAgency", parameters, commandType: CommandType.StoredProcedure);

            var agencyId = parameters.Get<int>("@Id");

            // Insertar programas
            if (agencyRequest.Programs != null && agencyRequest.Programs.Count > 0)
            {
                foreach (var programId in agencyRequest.Programs)
                {
                    await InsertAgencyProgram(agencyId, programId);
                }
            }

            // Invalidar caché
            InvalidateCache(agencyId);

            return agencyId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la agencia");
            throw;
        }
    }

    /// <summary>
    /// Inserta un programa de agencia
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="programId">Id del programa</param>
    /// <returns>True si se insertó correctamente</returns>
    public async Task<bool> InsertAgencyProgram(int agencyId, int programId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new { agencyId, programId };
            var rowsAffected = await dbConnection.QueryFirstOrDefaultAsync<int>("100_InsertAgencyProgram", parameters, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el programa de la agencia");
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
            var parameters = new DynamicParameters();
            parameters.Add("@Id", agencyId);
            parameters.Add("@Name", agencyRequest.Name);
            parameters.Add("@AgencyStatusId", agencyRequest.StatusId);
            parameters.Add("@SdrNumber", agencyRequest.SdrNumber);
            parameters.Add("@UieNumber", agencyRequest.UieNumber);
            parameters.Add("@EinNumber", agencyRequest.EinNumber);
            parameters.Add("@Address", agencyRequest.Address);
            parameters.Add("@ZipCode", agencyRequest.ZipCode);
            parameters.Add("@CityId", agencyRequest.CityId);
            parameters.Add("@RegionId", agencyRequest.RegionId);
            parameters.Add("@Latitude", agencyRequest.Latitude);
            parameters.Add("@Longitude", agencyRequest.Longitude);
            parameters.Add("@PostalAddress", agencyRequest.PostalAddress);
            parameters.Add("@PostalZipCode", agencyRequest.PostalZipCode);
            parameters.Add("@PostalCityId", agencyRequest.PostalCityId);
            parameters.Add("@PostalRegionId", agencyRequest.PostalRegionId);
            parameters.Add("@Phone", agencyRequest.Phone);
            parameters.Add("@ImageURL", agencyRequest.ImageUrl);
            parameters.Add("@Email", agencyRequest.Email);
            parameters.Add("@AgencyCode", agencyRequest.AgencyCode);
            parameters.Add("@NonProfit", agencyRequest.NonProfit);
            parameters.Add("@FederalFundsDenied", agencyRequest.FederalFundsDenied);
            parameters.Add("@StateFundsDenied", agencyRequest.StateFundsDenied);
            parameters.Add("@OrganizedAthleticPrograms", agencyRequest.OrganizedAthleticPrograms);
            parameters.Add("@AtRiskService", agencyRequest.AtRiskService);
            parameters.Add("@ServiceTime", agencyRequest.ServiceTime);
            parameters.Add("@TaxExemptionStatus", agencyRequest.TaxExemptionStatus);
            parameters.Add("@TaxExemptionType", agencyRequest.TaxExemptionType);

            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync("103_UpdateAgency", parameters, commandType: CommandType.StoredProcedure);

            // Invalidar caché
            InvalidateCache(agencyId);

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la agencia");
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
            _logger.LogError(ex, "Error al actualizar el logo de la agencia");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado de una agencia
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="statusId">Id del nuevo estado</param>
    /// <param name="rejectionJustification">Justificación para rechazo</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateAgencyStatus(int agencyId, int statusId, string? rejectionJustification)
    {
        try
        {
            _logger.LogInformation($"Actualizando estado de la agencia {agencyId} a {statusId}");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId);
            parameters.Add("@AgencyStatusId", statusId);
            parameters.Add("@rejectionJustification", rejectionJustification);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_UpdateAgencyStatus", parameters, commandType: CommandType.StoredProcedure);
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
                        ;
                    }
                }
            }

            // Invalidar caché
            InvalidateCache(agencyId);

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado de la agencia");
            throw;
        }
    }

    /// <summary>
    /// Actualiza el programa de una agencia
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="programId">Id del programa</param>
    /// <param name="statusId">Id del estado</param>
    /// <param name="comments">Comentarios</param>
    /// <param name="appointmentCoordinated">Indica si se coordinó la cita</param>
    /// <param name="appointmentDate">Fecha de la cita</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateAgencyProgram(int agencyId, int programId, int statusId, string userId, string comment, bool appointmentCoordinated, DateTime? appointmentDate)
    {
        try
        {
            _logger.LogInformation($"Actualizando el programa de la agencia {agencyId}");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@AgencyId", agencyId);
            parameters.Add("@ProgramId", programId);
            parameters.Add("@AgencyStatusId", statusId);
            parameters.Add("@UserId", userId);
            parameters.Add("@Comment", comment);
            parameters.Add("@AppointmentCoordinated", appointmentCoordinated);
            parameters.Add("@AppointmentDate", appointmentDate);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_UpdateAgencyProgram", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@ReturnValue");

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el programa de la agencia");
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
                InvalidateCache(agencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la agencia");
            throw new Exception(ex.Message);
        }
    }

    private void InvalidateCache(int? agencyId = null)
    {
        if (agencyId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Agency, agencyId));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.Agencies);
        _logger.LogInformation("Cache invalidado para Agency Repository");
    }

    // Métodos privados de mapeo
    private static DTOAgency MapAgencyFromResult(dynamic item)
    {
        return new DTOAgency
        {
            Id = item.Id,
            Name = item.Name,
            AgencyStatusId = item.AgencyStatusId,
            AgencyStatusName = item.AgencyStatusName,
            SdrNumber = item.SdrNumber,
            UieNumber = item.UieNumber,
            EinNumber = item.EinNumber,
            Address = item.Address,
            ZipCode = item.ZipCode,
            CityId = item.CityId,
            CityName = item.CityName,
            RegionId = item.RegionId,
            RegionName = item.RegionName,
            PostalAddress = item.PostalAddress,
            PostalZipCode = item.PostalZipCode,
            PostalCityId = item.PostalCityId,
            PostalCityName = item.PostalCityName,
            PostalRegionId = item.PostalRegionId,
            PostalRegionName = item.PostalRegionName,
            Latitude = item.Latitude,
            Longitude = item.Longitude,
            Phone = item.Phone,
            Email = item.Email,
            ImageUrl = item.ImageURL,
            NonProfit = item.NonProfit,
            FederalFundsDenied = item.FederalFundsDenied,
            StateFundsDenied = item.StateFundsDenied,
            OrganizedAthleticPrograms = item.OrganizedAthleticPrograms,
            AtRiskService = item.AtRiskService,
            ServiceTime = item.ServiceTime,
            TaxExemptionStatus = item.TaxExemptionStatus,
            TaxExemptionType = item.TaxExemptionType,
            RejectionJustification = item.RejectionJustification,
            Comment = item.Comment,
            AppointmentCoordinated = item.AppointmentCoordinated,
            AppointmentDate = item.AppointmentDate,
            IsActive = item.IsActive,
            IsListable = item.IsListable,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            AgencyCode = item.AgencyCode
        };
    }

    private static List<DTOProgram> MapProgramsFromResult(IEnumerable<dynamic> programs)
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
}