using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class AgencyRepository(IEmailService emailService, IPasswordService passwordService, DapperContext context, ILogger<AgencyRepository> logger) : IAgencyRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AgencyRepository> _logger = logger;
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly IPasswordService _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));

    /// <summary>
    /// Obtiene una agencia por su ID
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    public async Task<dynamic> GetAgencyById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo agencia por ID: {Id}", id);

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new { Id = id };

            var result = await dbConnection.QueryMultipleAsync("101_GetAgencyById", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            // Obtener los datos de la agencia
            var _agencyResult = await result.ReadFirstOrDefaultAsync<dynamic>();

            if (_agencyResult == null)
            {
                return null;
            }

            var agency = new DTOAgency
            {
                Id = _agencyResult.Id,
                Name = _agencyResult.Name,
                StatusId = _agencyResult.StatusId,
                // Datos de la Agencia
                SdrNumber = _agencyResult.SdrNumber,
                UieNumber = _agencyResult.UieNumber,
                EinNumber = _agencyResult.EinNumber,
                // Dirección y Teléfono
                Address = _agencyResult.Address,
                ZipCode = _agencyResult.ZipCode ?? 0,
                // Dirección Postal
                PostalAddress = _agencyResult.PostalAddress,
                PostalZipCode = _agencyResult.PostalZipCode ?? 0,
                // Teléfono
                Phone = _agencyResult.Phone,
                // Coordenadas
                Latitude = _agencyResult.Latitude,
                Longitude = _agencyResult.Longitude,
                // Datos del Contacto
                Email = _agencyResult.Email,
                CreatedAt = _agencyResult.CreatedAt,
                UpdatedAt = _agencyResult.UpdatedAt,
                // Imágen - Logo
                ImageURL = _agencyResult.ImageURL,
                // Relaciones
                City = new DTOCity
                {
                    Id = _agencyResult.CityId,
                    Name = _agencyResult.CityName
                },
                Region = new DTORegion
                {
                    Id = _agencyResult.RegionId,
                    Name = _agencyResult.RegionName
                },
                // Dirección Postal
                PostalCity = _agencyResult.PostalCityId != null ? new DTOCity
                {
                    Id = _agencyResult.PostalCityId,
                    Name = _agencyResult.PostalCityName
                } : null,
                PostalRegion = _agencyResult.PostalRegionId != null ? new DTORegion
                {
                    Id = _agencyResult.PostalRegionId,
                    Name = _agencyResult.PostalRegionName
                } : null,
                // Estatus
                Status = new DTOAgencyStatus
                {
                    Id = _agencyResult.StatusId,
                    Name = _agencyResult.StatusName
                },
                // Usuario
                User = new DTOUser
                {
                    Id = _agencyResult.UserId,
                    FirstName = _agencyResult.FirstName,
                    MiddleName = _agencyResult.MiddleName,
                    FatherLastName = _agencyResult.FatherLastName,
                    MotherLastName = _agencyResult.MotherLastName,
                    AdministrationTitle = _agencyResult.AdministrationTitle
                }
            };

            var _agenciesPrograms = await result.ReadAsync<dynamic>();

            if (_agenciesPrograms.Any())
            {
                // Obtener los programas de la agencia
                var programs = _agenciesPrograms.Select(item => new DTOProgram
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description
                }).ToList();

                // Asignar los programas a la agencia
                agency.Programs = programs;
            }

            return agency;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la agencia");
            throw new Exception(ex.Message);
        }
    }

    // <summary>
    /// Obtiene una agencia por su ID
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    public async Task<dynamic> GetAgencyByIdAndUserId(int agencyId, string userId)
    {
        try
        {
            _logger.LogInformation("Obteniendo agencia por ID: {AgencyId}", agencyId);

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new { agencyId, userId };

            var result = await dbConnection.QueryMultipleAsync("101_GetAgencyByIdAndUserId", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            // Obtener los datos de la agencia
            var _agencyResult = await result.ReadFirstOrDefaultAsync<dynamic>();

            if (_agencyResult == null)
            {
                return null;
            }

            var agency = new DTOAgency
            {
                Id = _agencyResult.Id,
                Name = _agencyResult.Name,
                StatusId = _agencyResult.StatusId,
                // Datos de la Agencia
                SdrNumber = _agencyResult.SdrNumber,
                UieNumber = _agencyResult.UieNumber,
                EinNumber = _agencyResult.EinNumber,
                // Dirección y Teléfono
                Address = _agencyResult.Address,
                ZipCode = _agencyResult.ZipCode ?? 0,
                // Dirección Postal
                PostalAddress = _agencyResult.PostalAddress,
                PostalZipCode = _agencyResult.PostalZipCode ?? 0,
                // Teléfono
                Phone = _agencyResult.Phone,
                // Coordenadas
                Latitude = _agencyResult.Latitude,
                Longitude = _agencyResult.Longitude,
                // Datos del Contacto
                Email = _agencyResult.Email,
                CreatedAt = _agencyResult.CreatedAt,
                UpdatedAt = _agencyResult.UpdatedAt,
                // Imágen - Logo
                ImageURL = _agencyResult.ImageURL,
                RejectionJustification = _agencyResult.RejectionJustification,
                // Relaciones
                City = new DTOCity
                {
                    Id = _agencyResult.CityId,
                    Name = _agencyResult.CityName
                },
                Region = new DTORegion
                {
                    Id = _agencyResult.RegionId,
                    Name = _agencyResult.RegionName
                },
                // Dirección Postal
                PostalCity = _agencyResult.PostalCityId != null ? new DTOCity
                {
                    Id = _agencyResult.PostalCityId,
                    Name = _agencyResult.PostalCityName
                } : null,
                PostalRegion = _agencyResult.PostalRegionId != null ? new DTORegion
                {
                    Id = _agencyResult.PostalRegionId,
                    Name = _agencyResult.PostalRegionName
                } : null,
                // Estatus
                Status = new DTOAgencyStatus
                {
                    Id = _agencyResult.StatusId,
                    Name = _agencyResult.StatusName
                },
                // Usuario
                User = new DTOUser
                {
                    Id = _agencyResult.UserId,
                    FirstName = _agencyResult.FirstName,
                    MiddleName = _agencyResult.MiddleName,
                    FatherLastName = _agencyResult.FatherLastName,
                    MotherLastName = _agencyResult.MotherLastName,
                    AdministrationTitle = _agencyResult.AdministrationTitle
                },
                Comment = _agencyResult.Comment ?? string.Empty,
                AppointmentCoordinated = _agencyResult.AppointmentCoordinated,
                AppointmentDate = _agencyResult.AppointmentDate
            };

            var _agenciesPrograms = await result.ReadAsync<dynamic>();

            if (_agenciesPrograms.Any())
            {
                // Obtener los programas de la agencia
                var programs = _agenciesPrograms.Select(item => new DTOProgram
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description
                }).ToList();

                // Asignar los programas a la agencia
                agency.Programs = programs;
            }

            return agency;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la agencia");
            throw new Exception(ex.Message);
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
    public async Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, string? userId, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las agencias de la base de datos");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new { take, skip, name, regionId, cityId, programId, statusId, userId, alls };

            var result = await dbConnection.QueryMultipleAsync("101_GetAgencies", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            // Obtener las agencias
            var agencies = result.Read<dynamic>().Select(item => new DTOAgency
            {
                Id = item.Id,
                Name = item.Name,
                StatusId = item.StatusId,
                // Datos de la Agencia
                SdrNumber = item.SdrNumber,
                UieNumber = item.UieNumber,
                EinNumber = item.EinNumber,
                // Dirección y Teléfono
                Address = item.Address,
                ZipCode = item.ZipCode,
                // Coordenadas
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                // Dirección Postal
                PostalAddress = item.PostalAddress,
                PostalZipCode = item.PostalZipCode ?? 0,
                // Teléfono
                Phone = item.Phone,
                // Datos del Contacto
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                // Imágen - Logo
                ImageURL = item.ImageURL,
                // Relaciones
                City = new DTOCity
                {
                    Id = item.CityId,
                    Name = item.CityName
                },
                Region = new DTORegion
                {
                    Id = item.RegionId,
                    Name = item.RegionName
                },
                PostalCity = item.PostalCityId != null ? new DTOCity
                {
                    Id = item.PostalCityId,
                    Name = item.PostalCityName
                } : null,
                PostalRegion = item.PostalRegionId != null ? new DTORegion
                {
                    Id = item.PostalRegionId,
                    Name = item.PostalRegionName
                } : null,
                Status = new DTOAgencyStatus
                {
                    Id = item.StatusId,
                    Name = item.StatusName
                },
                User = new DTOUser
                {
                    FirstName = item.FirstName,
                    MiddleName = item.MiddleName,
                    FatherLastName = item.FatherLastName,
                    MotherLastName = item.MotherLastName,
                    AdministrationTitle = item.AdministrationTitle
                },
                Comment = item.Comment ?? string.Empty,
                AppointmentCoordinated = item.AppointmentCoordinated,
                AppointmentDate = item.AppointmentDate
            }).ToList();

            // Obtener los programas de las agencias
            var _agenciesPrograms = await result.ReadAsync<dynamic>();

            // Asignar los programas a las agencias
            if (_agenciesPrograms.Any())
            {
                foreach (var agency in agencies)
                {
                    // Obtener los programas de la agencia
                    List<DTOProgram> programs = _agenciesPrograms.Where(ap => ap.AgencyId == agency.Id).Select(ap => new DTOProgram
                    {
                        Id = ap.Id,
                        Name = ap.Name,
                        Description = ap.Description,
                        AgencyId = ap.AgencyId
                    }).ToList();

                    agency.Programs = programs;

                    _logger.LogInformation("Agencia con programas: {AgencyWithPrograms}", agency.Id);
                }

                _logger.LogInformation("Agencias con programas: {AgenciesWithPrograms}", agencies.Count);

            }

            // Obtener el conteo de agencias
            var count = result.Read<int>().Single();

            // Retornar las agencias y el conteo
            return new { data = agencies, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las agencias");
            throw new Exception(ex.Message);
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
            var param = new { userId };
            var result = await dbConnection.QueryAsync<DTOProgram>("100_GetAgencyProgramsByUserId", param, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los programas de la agencia por el ID del usuario");
            throw new Exception(ex.Message);
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
            // Obtener todos los códigos existentes
            using IDbConnection db = _context.CreateConnection();
            var existingCodes = (await db.QueryAsync<string>("SELECT AgencyCode FROM Agency")).ToList();

            // Generar el nuevo código único
            string agencyCode = Utilities.GenerateAgencyCode(
                agencyRequest.Name,
                agencyRequest.Programs,
                existingCodes
            );

            var parameters = new DynamicParameters();
            parameters.Add("@Name", agencyRequest.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@AgencyStatusId", agencyRequest.StatusId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@AgencyCode", agencyCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@SdrNumber", agencyRequest.SdrNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UieNumber", agencyRequest.UieNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@EinNumber", agencyRequest.EinNumber, DbType.Int32, ParameterDirection.Input);
            // Dirección Física
            parameters.Add("@Address", agencyRequest.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@ZipCode", agencyRequest.ZipCode, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@CityId", agencyRequest.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RegionId", agencyRequest.RegionId, DbType.Int32, ParameterDirection.Input);
            // Coordenadas
            parameters.Add("@Latitude", agencyRequest.Latitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@Longitude", agencyRequest.Longitude, DbType.Double, ParameterDirection.Input);
            // Dirección Postal
            parameters.Add("@PostalAddress", agencyRequest.PostalAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("@PostalZipCode", agencyRequest.PostalZipCode, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PostalCityId", agencyRequest.PostalCityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PostalRegionId", agencyRequest.PostalRegionId, DbType.Int32, ParameterDirection.Input);
            // Teléfono
            parameters.Add("@Phone", agencyRequest.Phone, DbType.String, ParameterDirection.Input);
            // Datos del Contacto
            parameters.Add("@Email", agencyRequest.Email, DbType.String, ParameterDirection.Input);
            // Datos de elegibilidad
            parameters.Add("@NonProfit", agencyRequest.NonProfit, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@FederalFundsDenied", agencyRequest.FederalFundsDenied, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@StateFundsDenied", agencyRequest.StateFundsDenied, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@OrganizedAthleticPrograms", agencyRequest.OrganizedAthleticPrograms, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@AtRiskService", agencyRequest.AtRiskService, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("102_InsertAgency", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@Id");
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la agencia");
            throw new Exception(ex.Message);
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
            _logger.LogInformation($"Actualizando agencia {agencyId}");

            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", agencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Name", agencyRequest.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@AgencyStatusId", agencyRequest.StatusId, DbType.Int32, ParameterDirection.Input);
            // Datos de la Agencia
            parameters.Add("@SdrNumber", agencyRequest.SdrNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UieNumber", agencyRequest.UieNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@EinNumber", agencyRequest.EinNumber, DbType.Int32, ParameterDirection.Input);
            // Dirección Física
            parameters.Add("@Address", agencyRequest.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@ZipCode", agencyRequest.ZipCode, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@CityId", agencyRequest.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RegionId", agencyRequest.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Latitude", agencyRequest.Latitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@Longitude", agencyRequest.Longitude, DbType.Double, ParameterDirection.Input);
            // Dirección Postal
            parameters.Add("@PostalAddress", agencyRequest.PostalAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("@PostalZipCode", agencyRequest.PostalZipCode, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PostalCityId", agencyRequest.PostalCityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PostalRegionId", agencyRequest.PostalRegionId, DbType.Int32, ParameterDirection.Input);
            // Teléfono
            parameters.Add("@Phone", agencyRequest.Phone, DbType.String, ParameterDirection.Input);
            // Imágen - Logo
            parameters.Add("@ImageUrl", agencyRequest.ImageUrl, DbType.String, ParameterDirection.Input);
            // Datos del Contacto
            parameters.Add("@Email", agencyRequest.Email, DbType.String, ParameterDirection.Input);
            // Código de la agencia
            parameters.Add("@AgencyCode", agencyRequest.AgencyCode, DbType.String, ParameterDirection.Input);

            // Retorno
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("101_UpdateAgency", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@ReturnValue");

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la agencia");
            throw new Exception(ex.Message);
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
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la agencia");
            throw new Exception(ex.Message);
        }
    }
}