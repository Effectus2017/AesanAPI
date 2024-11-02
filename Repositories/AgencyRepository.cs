using System.Data;
using Api.Data;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class AgencyRepository(DapperContext context, ILogger<AgencyRepository> logger) : IAgencyRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AgencyRepository> _logger = logger;

    /// <summary>
    /// Obtiene todas las agencias de la base de datos
    /// </summary>
    /// <param name="take">El número de agencias a obtener</param>
    /// <param name="skip">El número de agencias a saltar</param>
    /// <param name="name">El nombre de la agencia</param>
    /// <param name="alls">Si se deben obtener todas las agencias</param>
    /// <returns>Las agencias</returns>
    public async Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las agencias de la base de datos");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new { take, skip, name, regionId, cityId, programId, statusId, alls };

            var result = await dbConnection.QueryMultipleAsync("100_GetAgencies", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

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
                PostalAddress = item.PostalAddress,
                Phone = item.Phone,
                // Coordenadas
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                // Datos del Contacto
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                // Imágen - Logo
                ImageURL = item.ImageURL,
                // Relaciones
                Program = new DTOProgram
                {
                    Id = item.ProgramId,
                    Name = item.ProgramName
                },
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
                }
            }).ToList();

            var count = result.Read<int>().Single();

            return new { data = agencies, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las agencias");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los estados de la agencia
    /// </summary>
    /// <param name="take">El número de estados a obtener</param>
    /// <param name="skip">El número de estados a saltar</param>
    /// <param name="name">El nombre del estado</param>
    /// <returns>Los estados de la agencia</returns>
    public async Task<dynamic> GetAllAgencyStatus(int take, int skip, string name, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los estados de la agencia");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new { take, skip, name, alls };

            var result = await dbConnection.QueryMultipleAsync("100_GetAllAgencyStatus", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var agencyStatus = result.Read<dynamic>().Select(item => new DTOAgencyStatus
            {
                Id = item.Id,
                Name = item.Name
            }).ToList();

            var count = result.Read<int>().Single();

            return new { data = agencyStatus, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los estados de la agencia");
            throw new Exception(ex.Message);
        }
    }

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

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>("100_GetAgencyById", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            return new DTOAgency
            {
                Id = result.Id,
                Name = result.Name,
                StatusId = result.StatusId,
                // Datos de la Agencia
                SdrNumber = result.SdrNumber,
                UieNumber = result.UieNumber,
                EinNumber = result.EinNumber,
                // Dirección y Teléfono
                Address = result.Address,
                ZipCode = result.ZipCode,
                PostalAddress = result.PostalAddress,
                Phone = result.Phone,
                // Coordenadas
                Latitude = result.Latitude,
                Longitude = result.Longitude,
                // Datos del Contacto
                Email = result.Email,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt,
                // Imágen - Logo
                ImageURL = result.ImageURL,
                // Relaciones
                Program = new DTOProgram
                {
                    Id = result.ProgramId,
                    Name = result.ProgramName
                },
                City = new DTOCity
                {
                    Id = result.CityId,
                    Name = result.CityName
                },
                Region = new DTORegion
                {
                    Id = result.RegionId,
                    Name = result.RegionName
                },
                Status = new DTOAgencyStatus
                {
                    Id = result.StatusId,
                    Name = result.StatusName
                },
                User = new DTOUser
                {
                    FirstName = result.FirstName,
                    MiddleName = result.MiddleName,
                    FatherLastName = result.FatherLastName,
                    MotherLastName = result.MotherLastName,
                    AdministrationTitle = result.AdministrationTitle
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la agencia");
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
            parameters.Add("@AgencyId", agencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Name", agencyRequest.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@StatusId", agencyRequest.StatusId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ProgramId", agencyRequest.ProgramId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SdrNumber", agencyRequest.SdrNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UieNumber", agencyRequest.UieNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@EinNumber", agencyRequest.EinNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@CityId", agencyRequest.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RegionId", agencyRequest.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Latitude", agencyRequest.Latitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@Longitude", agencyRequest.Longitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@Address", agencyRequest.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@ZipCode", agencyRequest.ZipCode, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PostalAddress", agencyRequest.PostalAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("@Phone", agencyRequest.Phone, DbType.String, ParameterDirection.Input);
            parameters.Add("@ImageUrl", agencyRequest.ImageUrl, DbType.String, ParameterDirection.Input);
            parameters.Add("@Email", agencyRequest.Email, DbType.String, ParameterDirection.Input);

            var rowsAffected = await dbConnection.QueryFirstOrDefaultAsync<int>("100_UpdateAgency", parameters, commandType: CommandType.StoredProcedure);
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
    public async Task<bool> UpdateAgencyStatus(int agencyId, int statusId, string rejectionJustification)
    {
        try
        {
            _logger.LogInformation($"Actualizando estado de la agencia {agencyId} a {statusId}");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new { agencyId, statusId, rejectionJustification };
            var rowsAffected = await dbConnection.QueryFirstOrDefaultAsync<int>("100_UpdateAgencyStatus", parameters, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado de la agencia");
            throw new Exception(ex.Message);
        }
    }
}